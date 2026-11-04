using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;

public sealed record GetRestaurantIngredientPricesForManageRequest(
    string? IngredientName,
    string? SortBy,
    string? SortOrder);

public sealed record RestaurantIngredientPriceForManageDto(
    Guid Id,
    Guid RestaurantId,
    Guid IngredientId,
    string IngredientName,
    decimal Price);

public sealed record GetRestaurantIngredientPricesForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<RestaurantIngredientPriceForManageDto> Items);

public sealed class GetRestaurantIngredientPricesForManageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetRestaurantIngredientPricesForManageResponse>(
                "api/menu/ingredient-prices/manage",
                HandleAsync)
            .WithName("GetRestaurantIngredientPricesForManageEndpoint")
            .WithTags("Ingredient Prices")
            .WithDescription("Returns restaurant ingredient prices for management.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetRestaurantIngredientPricesForManageRequest request,
        [AsParameters] PageParameters pageParameters,
        [FromServices] IRequestHandler<
            GetRestaurantIngredientPricesForManageQuery,
            GetRestaurantIngredientPricesForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetRestaurantIngredientPricesForManageQuery query = new(pageParameters, request);
        GetRestaurantIngredientPricesForManageResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
