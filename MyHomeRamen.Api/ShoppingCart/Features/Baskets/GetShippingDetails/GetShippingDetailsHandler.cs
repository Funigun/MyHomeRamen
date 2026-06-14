using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed class GetShippingDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetShippingDetailsQuery, ShippingDetailsResponse>
{
    public async Task<ShippingDetailsResponse> Handle(GetShippingDetailsQuery query, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.ShoppingCarts
            .GetByIdForUserWithShipping(query.BasketId, query.UserId)
            .FirstAsync(cancellationToken);

        ShippingAddressDto? addressDto = null;

        if (basket.ShippingDetails?.ShippingAddress is not null)
        {
            ShippingAddress? a = basket.ShippingDetails.ShippingAddress;
            addressDto = new ShippingAddressDto(a.Street, a.Building, a.Apartment, a.City, a.ZipCode);
        }

        return new ShippingDetailsResponse(
            basket.ShippingDetails?.PersonalPickup ?? false,
            basket.ShippingDetails?.Delivery ?? false,
            addressDto
        );
    }
}
