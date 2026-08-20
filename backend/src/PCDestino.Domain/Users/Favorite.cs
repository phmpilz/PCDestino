using PCDestino.Domain.Places;

namespace PCDestino.Domain.Users;

public sealed class Favorite
{
    private Favorite() { }

    private Favorite(string userId, Guid placeId, DateTimeOffset createdAt)
    {
        UserId = userId;
        PlaceId = placeId;
        CreatedAt = createdAt;
    }

    public string UserId { get; private set; } = string.Empty;
    public Guid PlaceId { get; private set; }
    public Place Place { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public static Favorite Create(string userId, Guid placeId, DateTimeOffset createdAt) => new(userId, placeId, createdAt);
}
