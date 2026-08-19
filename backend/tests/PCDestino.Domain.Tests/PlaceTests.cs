using PCDestino.Domain.Common;
using PCDestino.Domain.Places;

namespace PCDestino.Domain.Tests;

public sealed class PlaceTests
{
    [Fact]
    public void Create_WithInvalidCoordinates_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Place.Create(
            Guid.CreateVersion7(),
            "Local",
            "local",
            "Descrição válida",
            PlaceKind.Leisure,
            "user-1",
            DateTimeOffset.UtcNow,
            latitude: -91,
            longitude: -47));
    }

    [Fact]
    public void Publish_WithValidScore_ChangesPublicationState()
    {
        var now = DateTimeOffset.UtcNow;
        var place = Place.Create(
            Guid.CreateVersion7(),
            "Local",
            "local",
            "Descrição válida",
            PlaceKind.Leisure,
            "user-1",
            now);

        place.Publish(95, true, now);

        Assert.Equal(PublicationStatus.Published, place.Status);
        Assert.Equal(95, place.AccessibilityScore);
        Assert.True(place.IsVerified);
    }

    [Fact]
    public void Publish_WithInvalidScore_ThrowsDomainException()
    {
        var place = Place.Create(
            Guid.CreateVersion7(),
            "Local",
            "local",
            "Descrição válida",
            PlaceKind.Leisure,
            "user-1",
            DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => place.Publish(101, false, DateTimeOffset.UtcNow));
    }
}
