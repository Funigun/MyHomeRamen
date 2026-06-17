using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.ShippingDetails;

public class ShippingDetailsModel
{
    public bool PersonalPickup { get; set; }

    public bool Delivery { get; set; }

    public ShippingAddressModel? ShippingAddress { get; set; }

    public static ShippingDetailsModel FromResponse(ShippingDetailsResponse shippingDetailsResponse)
    {
        return new()
        {
            PersonalPickup = shippingDetailsResponse.PersonalPickup,
            Delivery = shippingDetailsResponse.Delivery,
            ShippingAddress = ShippingAddressModel.FromShippingDetailsDto(shippingDetailsResponse.ShippingAddress)
        };
    }
}
