using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

internal sealed class UpdatePaymentDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut("api/shopping-cart/{id}/update-payment-details", Handle)
                       .AllowAnonymous()
                       .WithTags("Baskets")
                       .WithDescription("Updates the payment details for active basket.");
    }

    internal static async Task<Results<Ok, BadRequest>> Handle(
            [FromRoute] Guid id,
            [FromBody] UpdatePaymentDetailsRequest request,
            [FromServices] ICommandHandler<UpdatePaymentDetailsCommand> handler,
            [FromServices] ICurrentUser currentUser,
            CancellationToken cancellationToken)
    {
        UpdatePaymentDetailsCommand command = new(new BasketId(id), new UserId(currentUser.UserId), request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Ok();
    }
}
