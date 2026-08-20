using PCDestino.Domain.Common;
using PCDestino.Domain.Places;

namespace PCDestino.Domain.Tests;

public sealed class ReviewTests
{
    [Theory]
    [InlineData(0, 5)]
    [InlineData(6, 5)]
    [InlineData(5, 0)]
    [InlineData(5, 6)]
    public void Create_WithInvalidRating_ThrowsDomainException(int rating, int accessibilityRating)
    {
        Assert.Throws<DomainException>(() => Review.Create(
            Guid.CreateVersion7(),
            "user-1",
            rating,
            accessibilityRating,
            "Comentário suficientemente longo.",
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_WithShortComment_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Review.Create(
            Guid.CreateVersion7(),
            "user-1",
            5,
            5,
            "Curto",
            DateTimeOffset.UtcNow));
    }
}
