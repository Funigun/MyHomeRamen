
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;

public record ShippingAddressDto(string Street, string Building, string Apartment, string City, string ZipCode);

public record UpdateShippingDetailsRequest(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);

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
        [FromServices] IRequestHandler<UpdateShippingDetailsCommand, Unit> handler,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken)
    {
        UpdateShippingDetailsCommand command = new(new BasketId(id), new UserId(currentUser.UserId), request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Ok();
    }
}

public static class Mappings
{
    public static ShippingDetails ToDomain(this UpdateShippingDetailsRequest request)
    {
        if (request.PersonalPickup)
        {
            return ShippingDetails.CreatePersonalPickup();
        }

        ShippingAddress address = new
        (
            request.ShippingAddress!.Street,
            request.ShippingAddress.Building,
            request.ShippingAddress.Apartment,
            request.ShippingAddress.City,
            request.ShippingAddress.ZipCode
        );

        return ShippingDetails.CreateDelivery(address);
    }
}
