using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;

public sealed record MenuCardForManageIngredientDto(Guid Id, string Name, bool IsCustom);

public sealed record MenuCardForManageOfferDto(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    string ImageUrl,
    decimal Price,
    string Status,
    IEnumerable<MenuCardForManageIngredientDto> Ingredients);

public sealed record MenuCardForManageSectionDto(
    Guid Id,
    string Name,
    int SortOrder,
    IEnumerable<MenuCardForManageOfferDto> Offers);

public sealed record GetMenuCardForManageResponse(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    IEnumerable<MenuCardForManageSectionDto> Sections);

public sealed class GetMenuCardForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetMenuCardForManageResponse>(
                "api/menu/cards/{id:guid}/manage",
                HandleAsync)
            .WithName("GetMenuCardForManageEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Returns active or inactive menu card details for restaurant management.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<GetMenuCardForManageQuery, GetMenuCardForManageResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetMenuCardForManageResponse? response = await handler.Handle(
            new GetMenuCardForManageQuery(id),
            cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
