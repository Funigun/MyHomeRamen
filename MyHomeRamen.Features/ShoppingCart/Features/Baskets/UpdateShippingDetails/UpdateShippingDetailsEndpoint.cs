using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;

internal sealed class UpdateShippingDetailsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut("api/shopping-cart/{id}/update-shipping-details", Handle)
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

