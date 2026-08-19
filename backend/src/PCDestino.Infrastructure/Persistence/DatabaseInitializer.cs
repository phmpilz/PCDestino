using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PCDestino.Domain.Catalog;
using PCDestino.Domain.Places;
using PCDestino.Domain.Users;

namespace PCDestino.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
        if (!options.RunMigrationsOnStartup)
        {
            return;
        }

        await MigrateScopeAsync(scope.ServiceProvider, options.SeedDemoData, cancellationToken);
    }

    public static async Task MigrateAsync(
        IServiceProvider services,
        bool seedDemoData = false,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        await MigrateScopeAsync(scope.ServiceProvider, seedDemoData, cancellationToken);
    }

    private static async Task MigrateScopeAsync(
        IServiceProvider services,
        bool seedDemoData,
        CancellationToken cancellationToken)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
        var dbContext = services.GetRequiredService<AppDbContext>();
        logger.LogInformation("Applying database migrations");
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (seedDemoData)
        {
            await SeedAsync(dbContext, cancellationToken);
            logger.LogInformation("Demo data is available");
        }
    }

    private static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Cities.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var campinas = City.Create("Campinas", "SP", "campinas-sp");
        var features = new[]
        {
            AccessibilityFeature.Create("step-free", "Entrada sem degraus", "Mobilidade"),
            AccessibilityFeature.Create("accessible-restroom", "Banheiro acessível", "Mobilidade"),
            AccessibilityFeature.Create("tactile-floor", "Piso tátil", "Visual"),
            AccessibilityFeature.Create("braille", "Informação em braile", "Visual"),
            AccessibilityFeature.Create("guide-dog", "Cão-guia bem-vindo", "Visual"),
            AccessibilityFeature.Create("accessible-parking", "Vaga acessível", "Mobilidade"),
            AccessibilityFeature.Create("sign-language", "Atendimento em Libras", "Auditiva")
        };
        dbContext.Cities.Add(campinas);
        dbContext.AccessibilityFeatures.AddRange(features);

        var marina = UserProfile.Create("demo-marina", "Marina Alves", campinas.Id, now);
        var joao = UserProfile.Create("demo-joao", "João Pedro", campinas.Id, now);
        dbContext.UserProfiles.AddRange(marina, joao);

        var park = Place.Create(
            campinas.Id,
            "Parque da Cidade",
            "parque-da-cidade",
            "Trilhas planas, banheiros adaptados e equipe treinada para receber todas as pessoas.",
            PlaceKind.Leisure,
            marina.ExternalId,
            now,
            -22.9056,
            -47.0608);
        park.SetContact("Av. Central, 100", "Centro", "13000-000", "(19) 3000-1000", "https://example.org/parque", now);
        park.AddAccessibilityFeature(features[0].Id, "Entrada principal nivelada");
        park.AddAccessibilityFeature(features[1].Id);
        park.AddAccessibilityFeature(features[2].Id);
        park.Publish(96, true, now);

        var cafe = Place.Create(
            campinas.Id,
            "Café Girassol",
            "cafe-girassol",
            "Entrada nivelada, mesas com circulação e cardápio acessível.",
            PlaceKind.Food,
            joao.ExternalId,
            now,
            -22.9021,
            -47.0562);
        cafe.SetContact("Rua das Flores, 45", "Jardins", "13010-010", "(19) 3000-2000", null, now);
        cafe.AddAccessibilityFeature(features[0].Id);
        cafe.AddAccessibilityFeature(features[3].Id);
        cafe.AddAccessibilityFeature(features[4].Id);
        cafe.Publish(92, false, now);

        dbContext.Places.AddRange(park, cafe);
        var review = Review.Create(park.Id, joao.ExternalId, 5, 5, "Ótima circulação e equipe realmente preparada.", now);
        review.Publish(now);
        dbContext.Reviews.Add(review);
        park.RecalculateRating([review]);

        dbContext.PointEvents.AddRange(
            PointEvent.Create(marina.ExternalId, campinas.Id, 2_840, PointEventType.ApprovedPlace, park.Id, now),
            PointEvent.Create(joao.ExternalId, campinas.Id, 2_610, PointEventType.ApprovedReview, review.Id, now));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
