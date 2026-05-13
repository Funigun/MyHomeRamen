using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Models;

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

    public IngredientRequestDto ToRequest() => new(Id, Quantity);
}
