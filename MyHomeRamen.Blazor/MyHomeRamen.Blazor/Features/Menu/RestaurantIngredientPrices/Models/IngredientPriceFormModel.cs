using MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Models;

public sealed class IngredientPriceFormModel
{
    public decimal Price { get; set; }

    public SetRestaurantIngredientPriceRequest ToRequest(Guid ingredientId) => new(ingredientId, Price);
}
