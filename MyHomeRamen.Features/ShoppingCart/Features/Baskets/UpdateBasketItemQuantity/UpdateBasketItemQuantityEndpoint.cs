using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateBasketItemQuantity;

public sealed class UpdateBasketItemQuantityEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut("api/shoppingcart/baskets/{basketId}/items/{basketItemId}/quantity", HandleAsync)
            .WithName("UpdateBasketItemQuantityEndpoint")
            .WithTags("Baskets")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid basketId,
        [FromRoute] Guid basketItemId,
        [FromBody] UpdateBasketItemQuantityRequest request,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IRequestHandler<UpdateBasketItemQuantityCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        UpdateBasketItemQuantityCommand command = new(
            new BasketId(basketId),
            new BasketItemId(basketItemId),
            new UserId(currentUser.UserId),
            request);

        await handler.Handle(command, cancellationToken);
        return Results.NoContent();
    }
}
