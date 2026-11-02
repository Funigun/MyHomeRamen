using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.CheckoutBasket;

public sealed class CheckoutBasketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPost<CheckoutBasketResponse>("api/shoppingcart/basket/checkout", HandleAsync)
            .WithName("CheckoutBasketEndpoint")
            .WithTags("Baskets")
            .WithDescription("Revalidates the active restaurant basket and creates an order from current prices.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CheckoutBasketRequest request,
        [FromServices] IRequestHandler<CheckoutBasketCommand, CheckoutBasketResponse?> handler,
        CancellationToken cancellationToken)
    {
        CheckoutBasketResponse? response = await handler.Handle(new CheckoutBasketCommand(request), cancellationToken);
        return response is null
            ? Results.NotFound()
            : Results.Created((string?)null, response);
    }
}
