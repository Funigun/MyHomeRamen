using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;

public sealed record MenuCardForManageOfferDto(
    Guid Id,
    Guid OfferId,
    string Name,
    int SortOrder,
    bool IsAvailable,
    decimal? Price);

public sealed record MenuCardForManageSectionDto(
    Guid Id,
    string Name,
    int SortOrder,
    IEnumerable<MenuCardForManageOfferDto> Offers);

public sealed record MenuCardOfferChoiceDto(Guid OfferId, string Name);

public sealed record GetMenuCardForManageResponse(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    bool IsActive,
    int SortOrder,
    MenuCardAllowedActions AllowedActions,
    IEnumerable<MenuCardForManageSectionDto> Sections,
    IEnumerable<MenuCardOfferChoiceDto> EligibleOffers);

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
