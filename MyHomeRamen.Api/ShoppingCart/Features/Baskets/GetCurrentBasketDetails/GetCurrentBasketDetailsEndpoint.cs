using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed class GetCurrentBasketDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketDetailsResponse>("api/shoppingcart/basket/summary", HandleAsync)
            .WithName("GetCurrentBasketDetailsEndpoint")
            .WithTags("Baskets")
            .WithDescription("Returns the active basket summary for the current user or guest.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCurrentBasketDetailsRequest, GetCurrentBasketDetailsResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketDetailsResponse? response = await handler.Handle(new GetCurrentBasketDetailsRequest(), cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
