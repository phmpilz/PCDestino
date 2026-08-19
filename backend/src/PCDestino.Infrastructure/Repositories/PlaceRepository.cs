using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PCDestino.Application.Common;
using PCDestino.Application.Places;
using PCDestino.Domain.Common;
using PCDestino.Domain.Places;
using PCDestino.Infrastructure.Persistence;

namespace PCDestino.Infrastructure.Repositories;

internal sealed class PlaceRepository(AppDbContext dbContext, TimeProvider timeProvider) : IPlaceRepository
{
    public async Task<PagedResult<PlaceSummaryDto>> SearchAsync(PlaceSearchQuery query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var radius = Math.Clamp(query.RadiusMeters, 100, 100_000);
        var places = dbContext.Places
            .AsNoTracking()
            .Where(place => place.Status == PublicationStatus.Published);

        if (query.CityId.HasValue)
        {
            places = places.Where(place => place.CityId == query.CityId.Value);
        }

        if (query.Kind.HasValue)
        {
            places = places.Where(place => place.Kind == query.Kind.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var escaped = query.Search.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
            var pattern = $"%{escaped}%";
            places = places.Where(place =>
                EF.Functions.ILike(place.Name, pattern, "\\") ||
                EF.Functions.ILike(place.Description, pattern, "\\") ||
                (place.Neighborhood != null && EF.Functions.ILike(place.Neighborhood, pattern, "\\")));
        }

        if (!string.IsNullOrWhiteSpace(query.AccessibilityFeature))
        {
            var featureCode = query.AccessibilityFeature.Trim().ToLowerInvariant();
            places = places.Where(place => place.AccessibilityFeatures.Any(feature =>
                feature.AccessibilityFeature.Code == featureCode));
        }

        Point? origin = null;
        if (query.Latitude.HasValue || query.Longitude.HasValue)
        {
            if (query.Latitude is not (>= -90 and <= 90) || query.Longitude is not (>= -180 and <= 180))
            {
                throw new DomainException("Latitude ou longitude inválida.");
            }

            origin = new Point(query.Longitude.Value, query.Latitude.Value) { SRID = 4326 };
            places = places.Where(place => place.Location != null && place.Location.Distance(origin) <= radius);
        }

        var total = await places.CountAsync(cancellationToken);
        var ordered = origin is null
            ? places.OrderByDescending(place => place.IsVerified).ThenByDescending(place => place.AccessibilityScore).ThenBy(place => place.Name)
            : places.OrderBy(place => place.Location!.Distance(origin)).ThenByDescending(place => place.AccessibilityScore);

        var entities = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(place => place.City)
            .Include(place => place.AccessibilityFeatures)
                .ThenInclude(item => item.AccessibilityFeature)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var items = entities.Select(place => new PlaceSummaryDto(
            place.Id,
            place.Name,
            place.Slug,
            place.Kind,
            place.City.Name,
            place.City.StateCode,
            place.Neighborhood,
            place.AverageRating,
            place.ReviewCount,
            place.AccessibilityScore,
            place.IsVerified,
            place.Location?.Y,
            place.Location?.X,
            origin is null || place.Location is null ? null : HaversineMeters(origin, place.Location),
            place.AccessibilityFeatures
                .OrderBy(item => item.AccessibilityFeature.Name)
                .Select(MapFeature)
                .ToArray()))
            .ToArray();

        return new PagedResult<PlaceSummaryDto>(items, page, pageSize, total);
    }

    public async Task<PlaceDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var place = await dbContext.Places
            .AsNoTracking()
            .Where(item => item.Id == id && item.Status == PublicationStatus.Published)
            .Include(item => item.City)
            .Include(item => item.AccessibilityFeatures)
                .ThenInclude(item => item.AccessibilityFeature)
            .Include(item => item.Reviews.Where(review => review.Status == PublicationStatus.Published))
            .AsSplitQuery()
            .SingleOrDefaultAsync(cancellationToken);

        if (place is null)
        {
            return null;
        }

        var userIds = place.Reviews.Select(review => review.UserId).Distinct().ToArray();
        var displayNames = await dbContext.UserProfiles
            .AsNoTracking()
            .Where(profile => userIds.Contains(profile.ExternalId))
            .ToDictionaryAsync(profile => profile.ExternalId, profile => profile.DisplayName, cancellationToken);

        return new PlaceDetailDto(
            place.Id,
            place.Name,
            place.Slug,
            place.Description,
            place.Kind,
            place.City.Name,
            place.City.StateCode,
            place.AddressLine,
            place.Neighborhood,
            place.PostalCode,
            place.Phone,
            place.Website,
            place.AverageRating,
            place.ReviewCount,
            place.AccessibilityScore,
            place.IsVerified,
            place.Location?.Y,
            place.Location?.X,
            place.AccessibilityFeatures.OrderBy(item => item.AccessibilityFeature.Name).Select(MapFeature).ToArray(),
            place.Reviews
                .OrderByDescending(review => review.CreatedAt)
                .Select(review => new ReviewDto(
                    review.Id,
                    displayNames.GetValueOrDefault(review.UserId, "Membro da comunidade"),
                    review.Rating,
                    review.AccessibilityRating,
                    review.Comment,
                    review.CreatedAt))
                .ToArray());
    }

    public async Task<CreatedResourceDto> CreateAsync(CreatePlaceCommand command, string userId, CancellationToken cancellationToken)
    {
        var cityExists = await dbContext.Cities.AnyAsync(city => city.Id == command.CityId && city.IsActive, cancellationToken);
        if (!cityExists)
        {
            throw new KeyNotFoundException("Cidade não encontrada.");
        }

        var requestedFeatures = command.AccessibilityFeatureIds.Distinct().ToArray();
        var validFeatures = await dbContext.AccessibilityFeatures
            .Where(feature => requestedFeatures.Contains(feature.Id) && feature.IsActive)
            .Select(feature => feature.Id)
            .ToListAsync(cancellationToken);

        if (validFeatures.Count != requestedFeatures.Length)
        {
            throw new DomainException("Um ou mais recursos de acessibilidade são inválidos.");
        }

        var baseSlug = Slug.From(command.Name);
        if (baseSlug.Length == 0)
        {
            throw new DomainException("Não foi possível gerar um identificador para o local.");
        }

        var slug = baseSlug;
        if (await dbContext.Places.AnyAsync(place => place.CityId == command.CityId && place.Slug == slug, cancellationToken))
        {
            var prefix = baseSlug[..Math.Min(baseSlug.Length, 191)];
            slug = $"{prefix}-{Guid.CreateVersion7():N}"[..Math.Min(prefix.Length + 9, 200)];
        }

        var now = timeProvider.GetUtcNow();
        var place = Place.Create(
            command.CityId,
            command.Name,
            slug,
            command.Description,
            command.Kind,
            userId,
            now,
            command.Latitude,
            command.Longitude);
        place.SetContact(command.AddressLine, command.Neighborhood, command.PostalCode, command.Phone, command.Website, now);
        foreach (var featureId in validFeatures)
        {
            place.AddAccessibilityFeature(featureId);
        }

        dbContext.Places.Add(place);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreatedResourceDto(place.Id, "pending");
    }

    private static PlaceFeatureDto MapFeature(PlaceAccessibilityFeature item) =>
        new(item.AccessibilityFeature.Id, item.AccessibilityFeature.Code, item.AccessibilityFeature.Name, item.AccessibilityFeature.Category, item.Evidence);

    private static double HaversineMeters(Point origin, Point destination)
    {
        const double earthRadius = 6_371_000;
        var latitude1 = DegreesToRadians(origin.Y);
        var latitude2 = DegreesToRadians(destination.Y);
        var latitudeDelta = DegreesToRadians(destination.Y - origin.Y);
        var longitudeDelta = DegreesToRadians(destination.X - origin.X);
        var a = Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2) +
                Math.Cos(latitude1) * Math.Cos(latitude2) *
                Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);
        return earthRadius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
