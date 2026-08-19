using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PCDestino.Domain.Catalog;
using PCDestino.Domain.Places;
using PCDestino.Domain.Users;

namespace PCDestino.Infrastructure.Persistence;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");
        builder.HasKey(city => city.Id);
        builder.Property(city => city.Name).HasMaxLength(120).IsRequired();
        builder.Property(city => city.StateCode).HasMaxLength(2).IsRequired();
        builder.Property(city => city.Slug).HasMaxLength(140).IsRequired();
        builder.HasIndex(city => city.Slug).IsUnique();
        builder.HasIndex(city => new { city.StateCode, city.Name });
    }
}

internal sealed class AccessibilityFeatureConfiguration : IEntityTypeConfiguration<AccessibilityFeature>
{
    public void Configure(EntityTypeBuilder<AccessibilityFeature> builder)
    {
        builder.ToTable("accessibility_features");
        builder.HasKey(feature => feature.Id);
        builder.Property(feature => feature.Code).HasMaxLength(80).IsRequired();
        builder.Property(feature => feature.Name).HasMaxLength(120).IsRequired();
        builder.Property(feature => feature.Category).HasMaxLength(80).IsRequired();
        builder.HasIndex(feature => feature.Code).IsUnique();
    }
}

internal sealed class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.ToTable("places");
        builder.HasKey(place => place.Id);
        builder.Property(place => place.Name).HasMaxLength(180).IsRequired();
        builder.Property(place => place.Slug).HasMaxLength(200).IsRequired();
        builder.Property(place => place.Description).HasMaxLength(2_000).IsRequired();
        builder.Property(place => place.Kind).HasConversion<string>().HasMaxLength(40);
        builder.Property(place => place.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(place => place.AddressLine).HasMaxLength(240);
        builder.Property(place => place.Neighborhood).HasMaxLength(120);
        builder.Property(place => place.PostalCode).HasMaxLength(16);
        builder.Property(place => place.Phone).HasMaxLength(32);
        builder.Property(place => place.Website).HasMaxLength(500);
        builder.Property(place => place.Location).HasColumnType("geography (point, 4326)");
        builder.Property(place => place.AverageRating).HasPrecision(3, 2);
        builder.Property(place => place.CreatedBy).HasMaxLength(120).IsRequired();
        builder.HasIndex(place => new { place.CityId, place.Slug }).IsUnique();
        builder.HasIndex(place => new { place.CityId, place.Status, place.Kind });
        builder.HasIndex(place => place.Location).HasMethod("gist");
        builder.HasOne(place => place.City).WithMany().HasForeignKey(place => place.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(place => place.AccessibilityFeatures)
            .WithOne(feature => feature.Place)
            .HasForeignKey(feature => feature.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(place => place.Reviews)
            .WithOne(review => review.Place)
            .HasForeignKey(review => review.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(place => place.AccessibilityFeatures).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(place => place.Reviews).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class PlaceAccessibilityFeatureConfiguration : IEntityTypeConfiguration<PlaceAccessibilityFeature>
{
    public void Configure(EntityTypeBuilder<PlaceAccessibilityFeature> builder)
    {
        builder.ToTable("place_accessibility_features");
        builder.HasKey(item => new { item.PlaceId, item.AccessibilityFeatureId });
        builder.Property(item => item.Evidence).HasMaxLength(1_000);
        builder.HasOne(item => item.AccessibilityFeature)
            .WithMany()
            .HasForeignKey(item => item.AccessibilityFeatureId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(item => item.AccessibilityFeatureId);
    }
}

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");
        builder.HasKey(review => review.Id);
        builder.Property(review => review.UserId).HasMaxLength(120).IsRequired();
        builder.Property(review => review.Comment).HasMaxLength(2_000).IsRequired();
        builder.Property(review => review.Status).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(review => new { review.PlaceId, review.UserId }).IsUnique();
        builder.HasIndex(review => new { review.Status, review.CreatedAt });
    }
}

internal sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.ExternalId).HasMaxLength(120).IsRequired();
        builder.Property(profile => profile.DisplayName).HasMaxLength(120).IsRequired();
        builder.HasIndex(profile => profile.ExternalId).IsUnique();
        builder.HasIndex(profile => new { profile.CityId, profile.ParticipateInRanking });
        builder.HasOne(profile => profile.City).WithMany().HasForeignKey(profile => profile.CityId).OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");
        builder.HasKey(favorite => new { favorite.UserId, favorite.PlaceId });
        builder.Property(favorite => favorite.UserId).HasMaxLength(120).IsRequired();
        builder.HasOne(favorite => favorite.Place).WithMany().HasForeignKey(favorite => favorite.PlaceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(favorite => new { favorite.UserId, favorite.CreatedAt });
    }
}

internal sealed class PointEventConfiguration : IEntityTypeConfiguration<PointEvent>
{
    public void Configure(EntityTypeBuilder<PointEvent> builder)
    {
        builder.ToTable("point_events");
        builder.HasKey(point => point.Id);
        builder.Property(point => point.UserId).HasMaxLength(120).IsRequired();
        builder.Property(point => point.Type).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(point => new { point.UserId, point.CityId, point.CreatedAt });
        builder.HasIndex(point => new { point.Type, point.ReferenceId }).IsUnique();
        builder.HasOne(point => point.City).WithMany().HasForeignKey(point => point.CityId).OnDelete(DeleteBehavior.Restrict);
    }
}
