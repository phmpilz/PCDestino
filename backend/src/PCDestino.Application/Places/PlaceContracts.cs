using PCDestino.Application.Common;
using PCDestino.Domain.Places;

namespace PCDestino.Application.Places;

public sealed record PlaceSearchQuery(
    Guid? CityId,
    string? Search,
    PlaceKind? Kind,
    string? AccessibilityFeature,
    double? Latitude,
    double? Longitude,
    int RadiusMeters = 10_000,
    int Page = 1,
    int PageSize = 20);

public sealed record PlaceFeatureDto(Guid Id, string Code, string Name, string Category, string? Evidence);

public sealed record PlaceSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    PlaceKind Kind,
    string City,
    string StateCode,
    string? Neighborhood,
    decimal AverageRating,
    int ReviewCount,
    int AccessibilityScore,
    bool IsVerified,
    double? Latitude,
    double? Longitude,
    double? DistanceMeters,
    IReadOnlyList<PlaceFeatureDto> AccessibilityFeatures);

public sealed record ReviewDto(
    Guid Id,
    string UserDisplayName,
    int Rating,
    int AccessibilityRating,
    string Comment,
    DateTimeOffset CreatedAt);

public sealed record PlaceDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    PlaceKind Kind,
    string City,
    string StateCode,
    string? AddressLine,
    string? Neighborhood,
    string? PostalCode,
    string? Phone,
    string? Website,
    decimal AverageRating,
    int ReviewCount,
    int AccessibilityScore,
    bool IsVerified,
    double? Latitude,
    double? Longitude,
    IReadOnlyList<PlaceFeatureDto> AccessibilityFeatures,
    IReadOnlyList<ReviewDto> Reviews);

public sealed record CreatePlaceCommand(
    Guid CityId,
    string Name,
    string Description,
    PlaceKind Kind,
    string? AddressLine,
    string? Neighborhood,
    string? PostalCode,
    string? Phone,
    string? Website,
    double? Latitude,
    double? Longitude,
    IReadOnlyCollection<Guid> AccessibilityFeatureIds);

public sealed record CreatedResourceDto(Guid Id, string Status);

public interface IPlaceRepository
{
    Task<PagedResult<PlaceSummaryDto>> SearchAsync(PlaceSearchQuery query, CancellationToken cancellationToken);
    Task<PlaceDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<CreatedResourceDto> CreateAsync(CreatePlaceCommand command, string userId, CancellationToken cancellationToken);
}
