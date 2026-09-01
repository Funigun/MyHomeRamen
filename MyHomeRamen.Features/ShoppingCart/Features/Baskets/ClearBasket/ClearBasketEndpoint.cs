using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.ClearBasket;

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
        [FromServices] IRequestHandler<ClearBasketCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        ClearBasketCommand command = new(basketId, currentUser.UserId);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
