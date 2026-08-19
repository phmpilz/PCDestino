using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PCDestino.Application.Catalog;
using PCDestino.Application.Community;
using PCDestino.Application.Moderation;
using PCDestino.Application.Places;
using PCDestino.Infrastructure.Persistence;
using PCDestino.Infrastructure.Repositories;

namespace PCDestino.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = DatabaseOptions.ResolveConnectionString(configuration);
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseNpgsql(
                    connectionString,
                    npgsql =>
                    {
                        npgsql.UseNetTopologySuite();
                        npgsql.CommandTimeout(databaseOptions.CommandTimeoutSeconds);
                        npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    })
                .UseSnakeCaseNamingConvention();
        }, poolSize: 128);

        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<ICommunityRepository, CommunityRepository>();
        services.AddScoped<IModerationRepository, ModerationRepository>();

        return services;
    }
}
