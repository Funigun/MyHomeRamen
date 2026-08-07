using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed record PaymentDetailsDto(string PaymentMethodId, string PaymentChannelId);

public sealed record PaymentDetailsResponse(string PaymentMethodId, string PaymentChannelId);

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

