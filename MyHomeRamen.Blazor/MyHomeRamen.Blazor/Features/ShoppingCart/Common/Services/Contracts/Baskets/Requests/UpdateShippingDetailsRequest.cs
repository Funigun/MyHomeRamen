using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Requests;

public record UpdateShippingDetailsRequest(bool PersonalPickup, bool Delivery, ShippingAddressDto? ShippingAddress);
