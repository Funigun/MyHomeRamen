namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

public sealed record GetCurrentBasketDetailsResponse(Guid Id, IEnumerable<BasketItemDto> Items);
