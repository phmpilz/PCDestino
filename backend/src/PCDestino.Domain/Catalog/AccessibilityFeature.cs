using PCDestino.Domain.Common;

namespace PCDestino.Domain.Catalog;

public sealed class AccessibilityFeature : Entity<Guid>
{
    private AccessibilityFeature() { }

    private AccessibilityFeature(string code, string name, string category)
    {
        Id = Guid.CreateVersion7();
        Code = code.Trim().ToLowerInvariant();
        Name = name.Trim();
        Category = category.Trim();

        if (Code.Length is 0 or > 80 || Name.Length is 0 or > 120 || Category.Length is 0 or > 80)
        {
            throw new DomainException("Recurso de acessibilidade inválido.");
        }
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public static AccessibilityFeature Create(string code, string name, string category) => new(code, name, category);
}
