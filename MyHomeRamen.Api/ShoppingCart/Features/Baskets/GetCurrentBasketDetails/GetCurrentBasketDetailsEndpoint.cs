using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed class GetCurrentBasketDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketDetailsResponse>("api/shoppingcart/basket/details", HandleAsync)
            .WithName("GetCurrentBasketDetailsEndpoint")
            .WithTags("Baskets")
            .WithDescription("Returns the active basket details for the current user or guest.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IQueryHandler<GetCurrentBasketDetailsQuery, GetCurrentBasketDetailsResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketDetailsQuery query = new();
        GetCurrentBasketDetailsResponse? response = await handler.Handle(query, cancellationToken);

        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
