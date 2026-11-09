using MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Models;

public sealed record RestaurantIngredientPriceViewModel(
    Guid IngredientId,
    string IngredientName,
    decimal? Price)
{
    public bool IsPriced => Price.HasValue;

    public static RestaurantIngredientPriceViewModel FromResponse(RestaurantIngredientPriceResponse response) =>
        new(response.IngredientId, response.IngredientName, response.Price);
}
