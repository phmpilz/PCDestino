using PCDestino.Domain.Catalog;
using PCDestino.Domain.Common;

namespace PCDestino.Domain.Users;

public enum PointEventType
{
    ApprovedPlace = 1,
    ApprovedReview = 2,
    VerifiedInformation = 3,
    Reversal = 4
}

public sealed class PointEvent : Entity<Guid>
{
    private PointEvent() { }

    private PointEvent(string userId, Guid cityId, int points, PointEventType type, Guid referenceId, DateTimeOffset createdAt)
    {
        if (points is < -10_000 or > 10_000 || points == 0)
        {
            throw new DomainException("Quantidade de pontos inválida.");
        }

        Id = Guid.CreateVersion7();
        UserId = userId;
        CityId = cityId;
        Points = points;
        Type = type;
        ReferenceId = referenceId;
        CreatedAt = createdAt;
    }

    public string UserId { get; private set; } = string.Empty;
    public Guid CityId { get; private set; }
    public City City { get; private set; } = null!;
    public int Points { get; private set; }
    public PointEventType Type { get; private set; }
    public Guid ReferenceId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static PointEvent Create(string userId, Guid cityId, int points, PointEventType type, Guid referenceId, DateTimeOffset createdAt) =>
        new(userId, cityId, points, type, referenceId, createdAt);
}
