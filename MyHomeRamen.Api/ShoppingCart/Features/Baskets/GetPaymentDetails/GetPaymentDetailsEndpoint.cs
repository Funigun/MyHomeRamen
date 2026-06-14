using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed class GetPaymentDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<PaymentDetailsResponse>("api/shopping-cart/{id}/payment-details", HandleAsync)
            .AllowAnonymous()
            .WithName("GetPaymentDetailsEndpoint")
            .WithTags("Baskets");
    }

    private static async Task<Results<Ok<PaymentDetailsResponse>, NotFound>> HandleAsync(
        [FromRoute(Name = "id")] Guid id,
        [FromServices] ICurrentUser currentUser,
        [FromServices] IQueryHandler<GetPaymentDetailsQuery, PaymentDetailsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetPaymentDetailsQuery query = new(new BasketId(id), new UserId(currentUser.UserId));
        PaymentDetailsResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
