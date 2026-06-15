using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;

public record UpdateShippingDetailsRequest(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);
