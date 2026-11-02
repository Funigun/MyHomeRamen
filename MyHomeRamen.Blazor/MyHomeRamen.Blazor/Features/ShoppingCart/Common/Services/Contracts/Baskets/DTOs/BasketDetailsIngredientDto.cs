namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

public sealed record BasketDetailsIngredientDto(Guid Id, Guid OriginalId, int Quantity, string Name, string Description, decimal Price);
