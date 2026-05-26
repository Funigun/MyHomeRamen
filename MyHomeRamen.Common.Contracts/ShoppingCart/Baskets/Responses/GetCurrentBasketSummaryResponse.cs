using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;

namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

public sealed record GetCurrentBasketSummaryResponse(Guid Id, IEnumerable<BasketSummaryItemDto> Items);
