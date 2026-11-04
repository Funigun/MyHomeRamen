using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Contracts;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed record CurrentBasketDetailsDto(
    Guid BasketId,
    Guid RestaurantId,
    IEnumerable<BasketDetailsItemDto> Items);

public sealed record GetCurrentBasketDetailsResponse(
    Guid BasketId,
    Guid RestaurantId,
    IEnumerable<BasketDetailsItemDto> Items,
    AllowedActionsDto AllowedActions);

public sealed record BasketDetailsItemDto(
    Guid Id,
    Guid RestaurantId,
    Guid OfferId,
    int Quantity,
    decimal Price,
    decimal? CurrentPrice,
    bool IsPriceChanged,
    bool IsAvailable,
    string? Comment,
    BasketDetailsItemProductDto Product);

public sealed record BasketDetailsItemProductDto(
    Guid Id,
    Guid OriginalId,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<BasketDetailsIngredientDto> BaseIngredients,
    IEnumerable<BasketDetailsIngredientDto> CustomIngredients);

public sealed record BasketDetailsIngredientDto(Guid Id, Guid OriginalId, int Quantity, string Name, string Description, decimal Price);

public sealed class GetCurrentBasketDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketDetailsResponse>("api/shopping-cart/basket/details", HandleAsync)
            .WithName("GetCurrentBasketDetailsEndpoint")
            .WithTags("Baskets")
            .WithDescription("Returns the active basket details for the current user or guest.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCurrentBasketDetailsQuery, GetCurrentBasketDetailsResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketDetailsQuery query = new();
        GetCurrentBasketDetailsResponse? response = await handler.Handle(query, cancellationToken);

        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
