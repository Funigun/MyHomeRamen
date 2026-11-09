namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

public sealed record GetRestaurantIngredientPricesForManageResponse(
    IEnumerable<RestaurantIngredientPriceResponse> Ingredients,
    IngredientPriceAllowedActions AllowedActions);

public sealed record RestaurantIngredientPriceResponse(
    Guid IngredientId,
    string IngredientName,
    decimal? Price);

public sealed record IngredientPriceAllowedActions(bool CanCreate, bool CanEdit, bool CanRemove);

public sealed record SetRestaurantIngredientPriceRequest(Guid IngredientId, decimal Price);
