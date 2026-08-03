using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed record GetShippingDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<ShippingDetailsResponse>;

public sealed class GetShippingDetailsHandler(IShoppingCartDbContext dbContext) : IQueryHandler<GetShippingDetailsQuery, ShippingDetailsResponse>
{
    public async Task<ShippingDetailsResponse> Handle(GetShippingDetailsQuery query, CancellationToken cancellationToken)
    {
        Basket basket = await dbContext.Basket.Query()
            .GetByIdForUserWithShippingAsync(query.BasketId, query.UserId, cancellationToken)
            ?? throw new InvalidOperationException("Basket was not found.");

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

