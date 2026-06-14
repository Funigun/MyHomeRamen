using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

public sealed record ShippingDetailsResponse(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);
