using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;

public sealed record GetRestaurantOffersForManageRequest(
    Guid? ProductId,
    string? ProductName,
    RestaurantProductOfferStatus? Status,
    string? SortBy,
    string? SortOrder);

public sealed record RestaurantProductOfferForManageDto(
    Guid Id,
    Guid RestaurantId,
    Guid ProductId,
    string ProductName,
    decimal Price,
    string Status,
    OfferAllowedActions AllowedActions)
{
    public Guid OfferId => Id;
}

public sealed record EligibleProductForOfferDto(Guid ProductId, string Name);

public sealed record OfferAllowedActions(bool CanCreate, bool CanEdit, bool CanRetire);

public sealed record GetRestaurantOffersForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyList<RestaurantProductOfferForManageDto> Items,
    IReadOnlyList<EligibleProductForOfferDto> EligibleProducts,
    OfferAllowedActions AllowedActions)
{
    public IReadOnlyList<RestaurantProductOfferForManageDto> Offers => Items;
}

public sealed class GetRestaurantOffersForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetRestaurantOffersForManageResponse>("api/menu/offers/manage", HandleAsync)
            .WithName("GetRestaurantOffersForManageEndpoint")
            .WithTags("Offers")
            .WithDescription("Returns active and retired restaurant offers for management.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetRestaurantOffersForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<GetRestaurantOffersForManageQuery, GetRestaurantOffersForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetRestaurantOffersForManageQuery query = new(pageParameters, request);
        GetRestaurantOffersForManageResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
