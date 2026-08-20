using NetTopologySuite.Geometries;
using PCDestino.Domain.Catalog;
using PCDestino.Domain.Common;

namespace PCDestino.Domain.Places;

public sealed class Place : Entity<Guid>
{
    private readonly List<PlaceAccessibilityFeature> _accessibilityFeatures = [];
    private readonly List<Review> _reviews = [];

    private Place() { }

    private Place(
        Guid cityId,
        string name,
        string slug,
        string description,
        PlaceKind kind,
        string createdBy,
        DateTimeOffset createdAt,
        Point? location)
    {
        Id = Guid.CreateVersion7();
        CityId = cityId;
        Name = Required(name, nameof(name), 180);
        Slug = Required(slug, nameof(slug), 200);
        Description = Required(description, nameof(description), 2_000);
        Kind = kind;
        CreatedBy = Required(createdBy, nameof(createdBy), 120);
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        Location = location;
        Status = PublicationStatus.Pending;
    }

    public Guid CityId { get; private set; }
    public City City { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public PlaceKind Kind { get; private set; }
    public string? AddressLine { get; private set; }
    public string? Neighborhood { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Phone { get; private set; }
    public string? Website { get; private set; }
    public Point? Location { get; private set; }
    public PublicationStatus Status { get; private set; }
    public bool IsVerified { get; private set; }
    public decimal AverageRating { get; private set; }
    public int ReviewCount { get; private set; }
    public int AccessibilityScore { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public IReadOnlyCollection<PlaceAccessibilityFeature> AccessibilityFeatures => _accessibilityFeatures;
    public IReadOnlyCollection<Review> Reviews => _reviews;

    public static Place Create(
        Guid cityId,
        string name,
        string slug,
        string description,
        PlaceKind kind,
        string createdBy,
        DateTimeOffset createdAt,
        double? latitude = null,
        double? longitude = null)
    {
        Point? point = null;
        if (latitude.HasValue || longitude.HasValue)
        {
            if (latitude is not (>= -90 and <= 90) || longitude is not (>= -180 and <= 180))
            {
                throw new DomainException("Latitude ou longitude inválida.");
            }

            point = new Point(longitude.Value, latitude.Value) { SRID = 4326 };
        }

        return new Place(cityId, name, slug, description, kind, createdBy, createdAt, point);
    }

    public void SetContact(
        string? addressLine,
        string? neighborhood,
        string? postalCode,
        string? phone,
        string? website,
        DateTimeOffset updatedAt)
    {
        AddressLine = Optional(addressLine, 240);
        Neighborhood = Optional(neighborhood, 120);
        PostalCode = Optional(postalCode, 16);
        Phone = Optional(phone, 32);
        Website = Optional(website, 500);
        UpdatedAt = updatedAt;
    }

    public void AddAccessibilityFeature(Guid featureId, string? evidence = null)
    {
        if (_accessibilityFeatures.Any(item => item.AccessibilityFeatureId == featureId))
        {
            return;
        }

        _accessibilityFeatures.Add(PlaceAccessibilityFeature.Create(Id, featureId, evidence));
    }

    public void Publish(int accessibilityScore, bool verified, DateTimeOffset now)
    {
        if (accessibilityScore is < 0 or > 100)
        {
            throw new DomainException("A pontuação de acessibilidade deve estar entre 0 e 100.");
        }

        Status = PublicationStatus.Published;
        AccessibilityScore = accessibilityScore;
        IsVerified = verified;
        VerifiedAt = verified ? now : null;
        UpdatedAt = now;
    }

    public void Reject(DateTimeOffset now)
    {
        Status = PublicationStatus.Rejected;
        UpdatedAt = now;
    }

    public void RecalculateRating(IEnumerable<Review> reviews)
    {
        var published = reviews.Where(review => review.Status == PublicationStatus.Published).ToArray();
        ReviewCount = published.Length;
        AverageRating = published.Length == 0 ? 0 : Math.Round((decimal)published.Average(review => review.Rating), 2);
    }

    private static string Required(string value, string field, int maxLength)
    {
        var normalized = value.Trim();
        if (normalized.Length is 0 || normalized.Length > maxLength)
        {
            throw new DomainException($"{field} deve ter entre 1 e {maxLength} caracteres.");
        }

        return normalized;
    }

    private static string? Optional(string? value, int maxLength)
    {
        var normalized = value?.Trim();
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"O valor deve ter no máximo {maxLength} caracteres.");
        }

        return normalized;
    }
}
