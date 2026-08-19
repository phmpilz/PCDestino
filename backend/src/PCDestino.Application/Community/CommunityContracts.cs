using PCDestino.Application.Common;

namespace PCDestino.Application.Community;

public sealed record CreateReviewCommand(int Rating, int AccessibilityRating, string Comment);

public sealed record FavoriteDto(Guid PlaceId, string Name, string City, int AccessibilityScore, DateTimeOffset CreatedAt);

public sealed record ProfileDto(
    string UserId,
    string DisplayName,
    Guid? CityId,
    string? City,
    bool ParticipateInRanking,
    int Points,
    int Reviews,
    int Contributions,
    int Favorites);

public sealed record UpdateProfileCommand(string DisplayName, Guid? CityId, bool ParticipateInRanking);

public sealed record LeaderboardEntryDto(int Position, string UserId, string DisplayName, int Points, int Contributions);

public interface ICommunityRepository
{
    Task<Guid> CreateReviewAsync(Guid placeId, CreateReviewCommand command, string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<FavoriteDto>> GetFavoritesAsync(string userId, CancellationToken cancellationToken);
    Task AddFavoriteAsync(Guid placeId, string userId, CancellationToken cancellationToken);
    Task RemoveFavoriteAsync(Guid placeId, string userId, CancellationToken cancellationToken);
    Task<ProfileDto> GetProfileAsync(string userId, string displayName, CancellationToken cancellationToken);
    Task<ProfileDto> UpdateProfileAsync(string userId, UpdateProfileCommand command, CancellationToken cancellationToken);
    Task<PagedResult<LeaderboardEntryDto>> GetLeaderboardAsync(Guid cityId, int page, int pageSize, CancellationToken cancellationToken);
}
