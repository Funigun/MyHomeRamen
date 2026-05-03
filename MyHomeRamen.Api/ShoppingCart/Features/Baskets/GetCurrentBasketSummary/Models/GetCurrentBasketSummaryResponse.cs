namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary.Models;

public sealed record GetCurrentBasketSummaryResponse(Guid Id, IEnumerable<BasketItemDto> Items);
