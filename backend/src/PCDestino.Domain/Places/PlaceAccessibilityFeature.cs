using PCDestino.Domain.Catalog;

namespace PCDestino.Domain.Places;

public sealed class PlaceAccessibilityFeature
{
    private PlaceAccessibilityFeature() { }

    private PlaceAccessibilityFeature(Guid placeId, Guid accessibilityFeatureId, string? evidence)
    {
        PlaceId = placeId;
        AccessibilityFeatureId = accessibilityFeatureId;
        Evidence = string.IsNullOrWhiteSpace(evidence) ? null : evidence.Trim();
    }

    public Guid PlaceId { get; private set; }
    public Place Place { get; private set; } = null!;
    public Guid AccessibilityFeatureId { get; private set; }
    public AccessibilityFeature AccessibilityFeature { get; private set; } = null!;
    public string? Evidence { get; private set; }

    internal static PlaceAccessibilityFeature Create(Guid placeId, Guid featureId, string? evidence) =>
        new(placeId, featureId, evidence);
}
