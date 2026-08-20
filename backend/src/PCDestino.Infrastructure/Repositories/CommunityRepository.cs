using Microsoft.EntityFrameworkCore;
using PCDestino.Application.Common;
using PCDestino.Application.Community;
using PCDestino.Domain.Common;
using PCDestino.Domain.Places;
using PCDestino.Domain.Users;
using PCDestino.Infrastructure.Persistence;

namespace PCDestino.Infrastructure.Repositories;

internal sealed class CommunityRepository(AppDbContext dbContext, TimeProvider timeProvider) : ICommunityRepository
{
    public async Task<Guid> CreateReviewAsync(
        Guid placeId,
        CreateReviewCommand command,
        string userId,
        CancellationToken cancellationToken)
    {
        var placeExists = await dbContext.Places.AnyAsync(
            place => place.Id == placeId && place.Status == PublicationStatus.Published,
            cancellationToken);
        if (!placeExists)
        {
            throw new KeyNotFoundException("Local não encontrado.");
        }

        var alreadyReviewed = await dbContext.Reviews.AnyAsync(
            review => review.PlaceId == placeId && review.UserId == userId,
            cancellationToken);
        if (alreadyReviewed)
        {
            throw new DomainException("Você já avaliou este local.");
        }

        var review = Review.Create(
            placeId,
            userId,
            command.Rating,
            command.AccessibilityRating,
            command.Comment,
            timeProvider.GetUtcNow());
        dbContext.Reviews.Add(review);
        await dbContext.SaveChangesAsync(cancellationToken);
        return review.Id;
    }

    public async Task<IReadOnlyList<FavoriteDto>> GetFavoritesAsync(string userId, CancellationToken cancellationToken) =>
        await dbContext.Favorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == userId && favorite.Place.Status == PublicationStatus.Published)
            .OrderByDescending(favorite => favorite.CreatedAt)
            .Select(favorite => new FavoriteDto(
                favorite.PlaceId,
                favorite.Place.Name,
                favorite.Place.City.Name,
                favorite.Place.AccessibilityScore,
                favorite.CreatedAt))
            .ToListAsync(cancellationToken);

    public async Task AddFavoriteAsync(Guid placeId, string userId, CancellationToken cancellationToken)
    {
        var placeExists = await dbContext.Places.AnyAsync(
            place => place.Id == placeId && place.Status == PublicationStatus.Published,
            cancellationToken);
        if (!placeExists)
        {
            throw new KeyNotFoundException("Local não encontrado.");
        }

        if (!await dbContext.Favorites.AnyAsync(favorite => favorite.UserId == userId && favorite.PlaceId == placeId, cancellationToken))
        {
            dbContext.Favorites.Add(Favorite.Create(userId, placeId, timeProvider.GetUtcNow()));
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveFavoriteAsync(Guid placeId, string userId, CancellationToken cancellationToken)
    {
        var favorite = await dbContext.Favorites.SingleOrDefaultAsync(
            item => item.UserId == userId && item.PlaceId == placeId,
            cancellationToken);
        if (favorite is null)
        {
            return;
        }

        dbContext.Favorites.Remove(favorite);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProfileDto> GetProfileAsync(string userId, string displayName, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, displayName, cancellationToken);
        return await BuildProfileAsync(profile, cancellationToken);
    }

    public async Task<ProfileDto> UpdateProfileAsync(
        string userId,
        UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        if (command.CityId.HasValue && !await dbContext.Cities.AnyAsync(city => city.Id == command.CityId && city.IsActive, cancellationToken))
        {
            throw new KeyNotFoundException("Cidade não encontrada.");
        }

        var profile = await dbContext.UserProfiles.SingleOrDefaultAsync(item => item.ExternalId == userId, cancellationToken);
        if (profile is null)
        {
            profile = UserProfile.Create(userId, command.DisplayName, command.CityId, timeProvider.GetUtcNow());
            profile.Update(command.DisplayName, command.CityId, command.ParticipateInRanking, timeProvider.GetUtcNow());
            dbContext.UserProfiles.Add(profile);
        }
        else
        {
            profile.Update(command.DisplayName, command.CityId, command.ParticipateInRanking, timeProvider.GetUtcNow());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await BuildProfileAsync(profile, cancellationToken);
    }

    public async Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(
        Guid cityId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var scores = dbContext.PointEvents
            .AsNoTracking()
            .Where(point => point.CityId == cityId)
            .GroupBy(point => point.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                Points = group.Sum(item => item.Points),
                Contributions = group.Count(item => item.Points > 0)
            });

        var leaderboard = from score in scores
                          join profile in dbContext.UserProfiles.AsNoTracking()
                              on score.UserId equals profile.ExternalId
                          where profile.ParticipateInRanking
                          orderby score.Points descending, profile.DisplayName
                          select new { score.UserId, profile.DisplayName, score.Points, score.Contributions };

        var total = await leaderboard.CountAsync(cancellationToken);
        var rawItems = await leaderboard
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var offset = (page - 1) * pageSize;
        var items = rawItems
            .Select((item, index) => new LeaderboardEntryDto(
                offset + index + 1,
                item.UserId,
                item.DisplayName,
                item.Points,
                item.Contributions))
            .ToArray();

        return new PagedResult<LeaderboardEntryDto>(items, page, pageSize, total);
    }

    private async Task<UserProfile> GetOrCreateProfileAsync(string userId, string displayName, CancellationToken cancellationToken)
    {
        var profile = await dbContext.UserProfiles.SingleOrDefaultAsync(item => item.ExternalId == userId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = UserProfile.Create(userId, displayName, null, timeProvider.GetUtcNow());
        dbContext.UserProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<ProfileDto> BuildProfileAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        var city = profile.CityId.HasValue
            ? await dbContext.Cities.AsNoTracking().Where(city => city.Id == profile.CityId).Select(city => city.Name).SingleOrDefaultAsync(cancellationToken)
            : null;
        var points = await dbContext.PointEvents.AsNoTracking().Where(item => item.UserId == profile.ExternalId).SumAsync(item => (int?)item.Points, cancellationToken) ?? 0;
        var reviews = await dbContext.Reviews.AsNoTracking().CountAsync(item => item.UserId == profile.ExternalId && item.Status == PublicationStatus.Published, cancellationToken);
        var contributions = await dbContext.Places.AsNoTracking().CountAsync(item => item.CreatedBy == profile.ExternalId && item.Status == PublicationStatus.Published, cancellationToken);
        var favorites = await dbContext.Favorites.AsNoTracking().CountAsync(item => item.UserId == profile.ExternalId, cancellationToken);

        return new ProfileDto(
            profile.ExternalId,
            profile.DisplayName,
            profile.CityId,
            city,
            profile.ParticipateInRanking,
            points,
            reviews,
            contributions,
            favorites);
    }
}
