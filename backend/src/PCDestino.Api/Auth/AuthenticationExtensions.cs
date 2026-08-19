using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace PCDestino.Api.Auth;

public static class AuthenticationExtensions
{
    public const string ModeratorPolicy = "Moderator";
    public const string AdminPolicy = "Admin";

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var settings = configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>()
            ?? new AuthenticationOptions();
        var developmentMode = string.Equals(settings.Mode, "Development", StringComparison.OrdinalIgnoreCase);
        if (developmentMode && !environment.IsDevelopment())
        {
            throw new InvalidOperationException("Authentication:Mode=Development só pode ser usado no ambiente Development.");
        }

        if (developmentMode)
        {
            services.AddAuthentication(DevelopmentAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, DevelopmentAuthenticationHandler>(
                    DevelopmentAuthenticationHandler.SchemeName,
                    _ => { });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(settings.Authority) || string.IsNullOrWhiteSpace(settings.ClientId))
            {
                throw new InvalidOperationException("Authentication:Authority e Authentication:ClientId são obrigatórios para Cognito.");
            }

            var authority = settings.Authority.TrimEnd('/');
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.RequireHttpsMetadata = true;
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authority,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.FromMinutes(1),
                        NameClaimType = "username",
                        RoleClaimType = "cognito:groups"
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var tokenUse = context.Principal?.FindFirst("token_use")?.Value;
                            var clientId = context.Principal?.FindFirst("client_id")?.Value;
                            if (tokenUse != "access" || !string.Equals(clientId, settings.ClientId, StringComparison.Ordinal))
                            {
                                context.Fail("Token Cognito inválido para esta API.");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
        }

        services.AddAuthorizationBuilder()
            .AddPolicy(ModeratorPolicy, policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
                HasGroup(context.User, "Moderator") || HasGroup(context.User, "Admin")))
            .AddPolicy(AdminPolicy, policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
                HasGroup(context.User, "Admin")));

        return services;
    }

    private static bool HasGroup(System.Security.Claims.ClaimsPrincipal user, string expected)
    {
        foreach (var claim in user.FindAll("cognito:groups"))
        {
            if (string.Equals(claim.Value, expected, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (claim.Value.StartsWith('['))
            {
                try
                {
                    var groups = JsonSerializer.Deserialize<string[]>(claim.Value) ?? [];
                    if (groups.Contains(expected, StringComparer.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (JsonException)
                {
                    // Claim malformed: authorization remains denied.
                }
            }
        }

        return false;
    }
}
