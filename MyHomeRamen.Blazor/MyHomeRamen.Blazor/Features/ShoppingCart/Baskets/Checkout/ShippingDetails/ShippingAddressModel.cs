using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.ShippingDetails;

public class ShippingAddressModel
{
    public string Street { get; set; } = string.Empty;

    public string Building { get; set; } = string.Empty;

    public string Apartment { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;

    public static ShippingAddressModel FromShippingDetailsDto(ShippingAddressDto? shippingAddressDto)
    {
        return shippingAddressDto == null
                                   ? new()
                                   : new()
                                     {
                                         Street = shippingAddressDto.Street,
                                         Building = shippingAddressDto.Building,
                                         Apartment = shippingAddressDto.Apartment,
                                         City = shippingAddressDto.City,
                                         ZipCode = shippingAddressDto.ZipCode
                                     };
    }
}
