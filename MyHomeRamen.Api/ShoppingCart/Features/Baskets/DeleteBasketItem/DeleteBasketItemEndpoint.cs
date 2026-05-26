using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

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
        [FromServices] ICommandHandler<DeleteBasketItemCommand> handler,
        CancellationToken cancellationToken)
    {
        DeleteBasketItemCommand command = new(new BasketId(basketId), new BasketItemId(basketItemId));
        await handler.Handle(command, cancellationToken);

        return Results.NoContent();
    }
}
