using System.Security.Claims;

namespace PCDestino.Api.Auth;

internal static class CurrentUser
{
    public static string Id(ClaimsPrincipal user) =>
        user.FindFirstValue("sub") ?? throw new UnauthorizedAccessException("Identificador do usuário ausente.");

    public static string DisplayName(ClaimsPrincipal user) =>
        user.FindFirstValue("name") ??
        user.FindFirstValue("username") ??
        user.FindFirstValue("email") ??
        "Membro da comunidade";
}
