using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdateShippingDetails;

internal sealed class UpdateShippingDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapStandardPut("api/shopping-cart/{id}/update-shipping-details", Handle)
           .AllowAnonymous()
           .WithTags("Baskets")
           .WithDescription("Updates the shipping details for active basket.");
    }

    internal static async Task<Results<Ok, BadRequest>> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdateShippingDetailsRequest request,
        [FromServices] ICommandHandler<UpdateShippingDetailsCommand> handler,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        UpdateShippingDetailsCommand command = new(new BasketId(id), new UserId(currentUser.UserId), request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Ok();
    }
}
