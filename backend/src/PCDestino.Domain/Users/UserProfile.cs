using PCDestino.Domain.Catalog;
using PCDestino.Domain.Common;

namespace PCDestino.Domain.Users;

public sealed class UserProfile : Entity<Guid>
{
    private UserProfile() { }

    private UserProfile(string externalId, string displayName, Guid? cityId, DateTimeOffset createdAt)
    {
        Id = Guid.CreateVersion7();
        ExternalId = externalId.Trim();
        DisplayName = displayName.Trim();
        CityId = cityId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public string ExternalId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public Guid? CityId { get; private set; }
    public City? City { get; private set; }
    public bool ParticipateInRanking { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static UserProfile Create(string externalId, string displayName, Guid? cityId, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(externalId) || externalId.Length > 120)
        {
            throw new DomainException("Identificador externo inválido.");
        }

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > 120)
        {
            throw new DomainException("Nome de exibição inválido.");
        }

        return new UserProfile(externalId, displayName, cityId, createdAt);
    }

    public void Update(string displayName, Guid? cityId, bool participateInRanking, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(displayName) || displayName.Trim().Length > 120)
        {
            throw new DomainException("Nome de exibição inválido.");
        }

        DisplayName = displayName.Trim();
        CityId = cityId;
        ParticipateInRanking = participateInRanking;
        UpdatedAt = now;
    }
}
