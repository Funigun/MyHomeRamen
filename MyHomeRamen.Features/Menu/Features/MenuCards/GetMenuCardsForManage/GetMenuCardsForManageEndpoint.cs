using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;

public sealed record GetMenuCardsForManageRequest(
    string? Name,
    bool? IsActive,
    string? SortBy,
    string? SortOrder);

public sealed record MenuCardForManageDto(
    Guid Id,
    Guid RestaurantId,
    string Name,
    bool IsActive);

public sealed record GetMenuCardsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<MenuCardForManageDto> Items);

public sealed class GetMenuCardsForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetMenuCardsForManageResponse>("api/menu/cards/manage", HandleAsync)
            .WithName("GetMenuCardsForManageEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Returns active and inactive menu cards for restaurant management.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetMenuCardsForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<GetMenuCardsForManageQuery, GetMenuCardsForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMenuCardsForManageQuery query = new(pageParameters, request);
        GetMenuCardsForManageResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
