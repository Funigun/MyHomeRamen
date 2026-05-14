using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

public sealed record GetCurrentBasketDetailsResponse(Guid Id, IEnumerable<BasketDetailsItemDto> Items);
