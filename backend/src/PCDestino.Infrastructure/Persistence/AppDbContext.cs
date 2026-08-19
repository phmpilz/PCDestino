using Microsoft.EntityFrameworkCore;
using PCDestino.Domain.Catalog;
using PCDestino.Domain.Places;
using PCDestino.Domain.Users;

namespace PCDestino.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<AccessibilityFeature> AccessibilityFeatures => Set<AccessibilityFeature>();
    public DbSet<Place> Places => Set<Place>();
    public DbSet<PlaceAccessibilityFeature> PlaceAccessibilityFeatures => Set<PlaceAccessibilityFeature>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<PointEvent> PointEvents => Set<PointEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
