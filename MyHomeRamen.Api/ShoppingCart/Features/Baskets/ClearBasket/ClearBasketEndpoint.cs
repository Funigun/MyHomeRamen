using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.ClearBasket;

public sealed class ClearBasketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardDelete("api/shoppingcart/baskets/{basketId}", HandleAsync)
            .WithName("ClearBasketEndpoint")
            .WithTags("Baskets")
            .AllowAnonymous();
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> HandleAsync(
        [FromRoute] Guid basketId,
        [FromServices] ICurrentUser currentUser,
        [FromServices] ICommandHandler<ClearBasketCommand> handler,
        CancellationToken cancellationToken)
    {
        ClearBasketCommand command = new(basketId, currentUser.UserId);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
