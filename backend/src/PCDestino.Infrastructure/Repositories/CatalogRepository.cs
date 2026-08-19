using Microsoft.EntityFrameworkCore;
using PCDestino.Application.Catalog;
using PCDestino.Infrastructure.Persistence;

namespace PCDestino.Infrastructure.Repositories;

internal sealed class CatalogRepository(AppDbContext dbContext) : ICatalogRepository
{
    public async Task<IReadOnlyList<CityDto>> GetCitiesAsync(CancellationToken cancellationToken) =>
        await dbContext.Cities
            .AsNoTracking()
            .Where(city => city.IsActive)
            .OrderBy(city => city.StateCode)
            .ThenBy(city => city.Name)
            .Select(city => new CityDto(city.Id, city.Name, city.StateCode, city.Slug))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AccessibilityFeatureDto>> GetAccessibilityFeaturesAsync(CancellationToken cancellationToken) =>
        await dbContext.AccessibilityFeatures
            .AsNoTracking()
            .Where(feature => feature.IsActive)
            .OrderBy(feature => feature.Category)
            .ThenBy(feature => feature.Name)
            .Select(feature => new AccessibilityFeatureDto(feature.Id, feature.Code, feature.Name, feature.Category))
            .ToListAsync(cancellationToken);
}
