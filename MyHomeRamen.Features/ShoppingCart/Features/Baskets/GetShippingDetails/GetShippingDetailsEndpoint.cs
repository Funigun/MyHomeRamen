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

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed record ShippingDetailsDto(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);

public sealed record ShippingAddressDto(string Street, string Building, string Apartment, string City, string ZipCode);

public sealed record ShippingDetailsResponse(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);

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

