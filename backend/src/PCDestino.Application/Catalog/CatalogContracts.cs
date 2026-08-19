namespace PCDestino.Application.Catalog;

public sealed record CityDto(Guid Id, string Name, string StateCode, string Slug);

public sealed record AccessibilityFeatureDto(Guid Id, string Code, string Name, string Category);

public interface ICatalogRepository
{
    Task<IReadOnlyList<CityDto>> GetCitiesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<AccessibilityFeatureDto>> GetAccessibilityFeaturesAsync(CancellationToken cancellationToken);
}
