namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Responses;

public sealed record BasketItemResponse(Guid Id, string ProductName, string ProductImageUrl, int Quantity, decimal Price);
