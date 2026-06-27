using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed class GetShippingDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<ShippingDetailsResponse>("api/shopping-cart/{id}/shipping-details", HandleAsync)
            .AllowAnonymous()
            .WithName("GetShippingDetailsEndpoint")
            .WithTags("Baskets");
    }

    private static async Task<Results<Ok<ShippingDetailsResponse>, NotFound>> HandleAsync(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IQueryHandler<GetShippingDetailsQuery, ShippingDetailsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetShippingDetailsQuery query = new(new BasketId(id), new UserId(currentUser.UserId));
        ShippingDetailsResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
