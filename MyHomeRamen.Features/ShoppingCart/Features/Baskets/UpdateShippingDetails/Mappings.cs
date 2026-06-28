using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.UpdateShippingDetails;

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

