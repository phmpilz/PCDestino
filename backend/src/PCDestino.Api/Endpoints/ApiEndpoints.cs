using System.Security.Claims;
using PCDestino.Api.Auth;
using PCDestino.Application.Catalog;
using PCDestino.Application.Community;
using PCDestino.Application.Moderation;
using PCDestino.Application.Places;
using PCDestino.Domain.Places;

namespace PCDestino.Api.Endpoints;

internal static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api/v1");
        MapCatalog(api);
        MapPlaces(api);
        MapCommunity(api);
        MapModeration(api);
        return endpoints;
    }

    private static void MapCatalog(RouteGroupBuilder api)
    {
        var catalog = api.MapGroup("/catalog").WithTags("Catálogo");
        catalog.MapGet("/cities", async (ICatalogRepository repository, CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetCitiesAsync(cancellationToken)))
            .AllowAnonymous()
            .CacheOutput("catalog")
            .WithSummary("Lista as cidades ativas");
        catalog.MapGet("/accessibility-features", async (ICatalogRepository repository, CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetAccessibilityFeaturesAsync(cancellationToken)))
            .AllowAnonymous()
            .CacheOutput("catalog")
            .WithSummary("Lista os recursos de acessibilidade padronizados");
    }

    private static void MapPlaces(RouteGroupBuilder api)
    {
        var places = api.MapGroup("/places").WithTags("Locais e serviços");
        places.MapGet("/", async (
                Guid? cityId,
                string? search,
                PlaceKind? kind,
                string? accessibilityFeature,
                double? latitude,
                double? longitude,
                int? radiusMeters,
                int? page,
                int? pageSize,
                IPlaceRepository repository,
                CancellationToken cancellationToken) =>
            {
                var query = new PlaceSearchQuery(
                    cityId,
                    search,
                    kind,
                    accessibilityFeature,
                    latitude,
                    longitude,
                    radiusMeters ?? 10_000,
                    page ?? 1,
                    pageSize ?? 20);
                return Results.Ok(await repository.SearchAsync(query, cancellationToken));
            })
            .AllowAnonymous()
            .CacheOutput()
            .WithSummary("Pesquisa locais e serviços publicados");
        places.MapGet("/{id:guid}", async (Guid id, IPlaceRepository repository, CancellationToken cancellationToken) =>
            {
                var place = await repository.GetByIdAsync(id, cancellationToken);
                return place is null ? Results.NotFound() : Results.Ok(place);
            })
            .AllowAnonymous()
            .CacheOutput()
            .WithSummary("Obtém os detalhes de um local ou serviço");
        places.MapPost("/", async (
                CreatePlaceCommand command,
                ClaimsPrincipal user,
                IPlaceRepository repository,
                CancellationToken cancellationToken) =>
            {
                var created = await repository.CreateAsync(command, CurrentUser.Id(user), cancellationToken);
                return Results.Created($"/api/v1/places/{created.Id}", created);
            })
            .RequireAuthorization()
            .WithSummary("Envia um local ou serviço para moderação");
        places.MapPost("/{id:guid}/reviews", async (
                Guid id,
                CreateReviewCommand command,
                ClaimsPrincipal user,
                ICommunityRepository repository,
                CancellationToken cancellationToken) =>
            {
                var reviewId = await repository.CreateReviewAsync(id, command, CurrentUser.Id(user), cancellationToken);
                return Results.Accepted($"/api/v1/places/{id}", new { id = reviewId, status = "pending" });
            })
            .RequireAuthorization()
            .WithSummary("Envia uma avaliação para moderação");
    }

    private static void MapCommunity(RouteGroupBuilder api)
    {
        var community = api.MapGroup("/community").WithTags("Comunidade");
        community.MapGet("/leaderboard/{cityId:guid}", async (
                Guid cityId,
                int? page,
                int? pageSize,
                ICommunityRepository repository,
                CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetLeaderboardAsync(cityId, page ?? 1, pageSize ?? 20, cancellationToken)))
            .AllowAnonymous()
            .CacheOutput()
            .WithSummary("Obtém o ranking público de uma cidade");

        var me = api.MapGroup("/me").WithTags("Perfil").RequireAuthorization();
        me.MapGet("/", async (ClaimsPrincipal user, ICommunityRepository repository, CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetProfileAsync(
                    CurrentUser.Id(user),
                    CurrentUser.DisplayName(user),
                    cancellationToken)))
            .WithSummary("Obtém ou cria o perfil do usuário autenticado");
        me.MapPut("/", async (
                UpdateProfileCommand command,
                ClaimsPrincipal user,
                ICommunityRepository repository,
                CancellationToken cancellationToken) =>
                Results.Ok(await repository.UpdateProfileAsync(CurrentUser.Id(user), command, cancellationToken)))
            .WithSummary("Atualiza o perfil do usuário autenticado");
        me.MapGet("/favorites", async (ClaimsPrincipal user, ICommunityRepository repository, CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetFavoritesAsync(CurrentUser.Id(user), cancellationToken)))
            .WithSummary("Lista os favoritos do usuário autenticado");
        me.MapPut("/favorites/{placeId:guid}", async (
                Guid placeId,
                ClaimsPrincipal user,
                ICommunityRepository repository,
                CancellationToken cancellationToken) =>
            {
                await repository.AddFavoriteAsync(placeId, CurrentUser.Id(user), cancellationToken);
                return Results.NoContent();
            })
            .WithSummary("Adiciona um local aos favoritos");
        me.MapDelete("/favorites/{placeId:guid}", async (
                Guid placeId,
                ClaimsPrincipal user,
                ICommunityRepository repository,
                CancellationToken cancellationToken) =>
            {
                await repository.RemoveFavoriteAsync(placeId, CurrentUser.Id(user), cancellationToken);
                return Results.NoContent();
            })
            .WithSummary("Remove um local dos favoritos");
    }

    private static void MapModeration(RouteGroupBuilder api)
    {
        var moderation = api.MapGroup("/moderation")
            .WithTags("Moderação")
            .RequireAuthorization(AuthenticationExtensions.ModeratorPolicy);
        moderation.MapGet("/queue", async (int? take, IModerationRepository repository, CancellationToken cancellationToken) =>
                Results.Ok(await repository.GetQueueAsync(take ?? 50, cancellationToken)))
            .WithSummary("Lista a fila de moderação");
        moderation.MapPost("/places/{id:guid}", async (
                Guid id,
                ModeratePlaceCommand command,
                IModerationRepository repository,
                CancellationToken cancellationToken) =>
            {
                await repository.ModeratePlaceAsync(id, command, cancellationToken);
                return Results.NoContent();
            })
            .WithSummary("Aprova ou rejeita um local");
        moderation.MapPost("/reviews/{id:guid}", async (
                Guid id,
                ModerateReviewCommand command,
                IModerationRepository repository,
                CancellationToken cancellationToken) =>
            {
                await repository.ModerateReviewAsync(id, command, cancellationToken);
                return Results.NoContent();
            })
            .WithSummary("Aprova ou rejeita uma avaliação");
    }
}
