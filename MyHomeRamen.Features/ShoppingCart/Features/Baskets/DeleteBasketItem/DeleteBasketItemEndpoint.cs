using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardDelete("api/shoppingcart/baskets/{basketId}/items/{basketItemId}", HandleAsync)
            .WithName("DeleteBasketItemEndpoint")
            .WithDescription("Removes a specific item from the basket.")
            .WithTags("Baskets")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid basketId,
        [FromRoute] Guid basketItemId,
        [FromServices] IRequestHandler<DeleteBasketItemCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        DeleteBasketItemCommand command = new(new BasketId(basketId), new BasketItemId(basketItemId));
        await handler.Handle(command, cancellationToken);

        return Results.NoContent();
    }
}
