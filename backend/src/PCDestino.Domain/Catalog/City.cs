using PCDestino.Domain.Common;

namespace PCDestino.Domain.Catalog;

public sealed class City : Entity<Guid>
{
    private City() { }

    private City(string name, string stateCode, string slug)
    {
        Id = Guid.CreateVersion7();
        Name = Required(name, nameof(name), 120);
        StateCode = Required(stateCode, nameof(stateCode), 2).ToUpperInvariant();
        Slug = Required(slug, nameof(slug), 140);
    }

    public string Name { get; private set; } = string.Empty;
    public string StateCode { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public static City Create(string name, string stateCode, string slug) => new(name, stateCode, slug);

    private static string Required(string value, string field, int maxLength)
    {
        var normalized = value.Trim();
        if (normalized.Length is 0 || normalized.Length > maxLength)
        {
            throw new DomainException($"{field} deve ter entre 1 e {maxLength} caracteres.");
        }

        return normalized;
    }
}
