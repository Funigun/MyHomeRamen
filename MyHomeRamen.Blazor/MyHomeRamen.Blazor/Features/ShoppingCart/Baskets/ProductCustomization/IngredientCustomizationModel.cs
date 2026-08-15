using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;
using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Checkout.BasketDetails;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.ProductCustomization;

public sealed class IngredientCustomizationModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public bool IsSelected { get; set; } = true;

    public int Quantity { get; set; } = 1;

    public static IngredientCustomizationModel FromDto(IngredientDto dto, bool selectedByDefault = true) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsSelected = selectedByDefault,
            Quantity = 1
        };

    public BasketIngredientDto ToRequest() => new(Id, Quantity);

    public static IngredientCustomizationModel FromBasketDetailsDto(CheckoutIngredientModel dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsSelected = true,
            Quantity = 1
        };
}
