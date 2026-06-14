namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

public sealed record ShippingAddressDto(string Street, string Building, string Apartment, string City, string ZipCode);