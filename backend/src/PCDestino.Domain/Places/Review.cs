using PCDestino.Domain.Common;

namespace PCDestino.Domain.Places;

public sealed class Review : Entity<Guid>
{
    private Review() { }

    private Review(Guid placeId, string userId, int rating, int accessibilityRating, string comment, DateTimeOffset createdAt)
    {
        if (rating is < 1 or > 5 || accessibilityRating is < 1 or > 5)
        {
            throw new DomainException("As notas devem estar entre 1 e 5.");
        }

        var normalizedComment = comment.Trim();
        if (normalizedComment.Length is < 10 or > 2_000)
        {
            throw new DomainException("O comentário deve ter entre 10 e 2000 caracteres.");
        }

        Id = Guid.CreateVersion7();
        PlaceId = placeId;
        UserId = userId.Trim();
        Rating = rating;
        AccessibilityRating = accessibilityRating;
        Comment = normalizedComment;
        Status = PublicationStatus.Pending;
        CreatedAt = createdAt;
    }

    public Guid PlaceId { get; private set; }
    public Place Place { get; private set; } = null!;
    public string UserId { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public int AccessibilityRating { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public PublicationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ModeratedAt { get; private set; }

    public static Review Create(Guid placeId, string userId, int rating, int accessibilityRating, string comment, DateTimeOffset createdAt) =>
        new(placeId, userId, rating, accessibilityRating, comment, createdAt);

    public void Publish(DateTimeOffset now)
    {
        Status = PublicationStatus.Published;
        ModeratedAt = now;
    }

    public void Reject(DateTimeOffset now)
    {
        Status = PublicationStatus.Rejected;
        ModeratedAt = now;
    }
}
