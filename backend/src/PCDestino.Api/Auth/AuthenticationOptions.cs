namespace PCDestino.Api.Auth;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";
    public string Mode { get; init; } = "Cognito";
    public string? Authority { get; init; }
    public string? ClientId { get; init; }
}
