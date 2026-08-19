using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PCDestino.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string? Host { get; init; }
    public int Port { get; init; } = 5432;
    public string Name { get; init; } = "pcdestino";
    public string? Username { get; init; }
    public string? Password { get; init; }
    public bool RequireSsl { get; init; }
    public int MaxPoolSize { get; init; } = 100;
    public int CommandTimeoutSeconds { get; init; } = 30;
    public bool RunMigrationsOnStartup { get; init; }
    public bool SeedDemoData { get; init; }

    public static string ResolveConnectionString(IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured;
        }

        var options = configuration.GetSection(SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();
        if (string.IsNullOrWhiteSpace(options.Host) ||
            string.IsNullOrWhiteSpace(options.Username) ||
            string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings__Default ou Database__Host, Database__Username e Database__Password.");
        }

        return new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Name,
            Username = options.Username,
            Password = options.Password,
            SslMode = options.RequireSsl ? SslMode.Require : SslMode.Prefer,
            MaxPoolSize = options.MaxPoolSize,
            CommandTimeout = options.CommandTimeoutSeconds,
            Timeout = 15,
            KeepAlive = 30,
            ApplicationName = "PCDestino.Api"
        }.ConnectionString;
    }
}
