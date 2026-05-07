namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Responses;

public sealed record GetCurrentBasketSummaryResponse(Guid Id, IEnumerable<BasketItemResponse> Items);
