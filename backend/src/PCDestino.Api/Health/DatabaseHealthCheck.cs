using Microsoft.Extensions.Diagnostics.HealthChecks;
using PCDestino.Infrastructure.Persistence;

namespace PCDestino.Api.Health;

internal sealed class DatabaseHealthCheck(AppDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("PostgreSQL indisponível.");
}
