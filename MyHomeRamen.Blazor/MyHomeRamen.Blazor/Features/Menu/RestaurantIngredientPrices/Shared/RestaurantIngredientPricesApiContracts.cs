namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

public sealed record GetRestaurantIngredientPricesForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<RestaurantIngredientPriceResponse> Items,
    IngredientPriceAllowedActions AllowedActions);

public sealed record RestaurantIngredientPriceResponse(
    Guid Id,
    Guid RestaurantId,
    Guid IngredientId,
    string IngredientName,
    decimal? Price);

public sealed record IngredientPriceAllowedActions(bool CanCreate, bool CanEdit, bool CanRemove);

public sealed record SetRestaurantIngredientPriceRequest(Guid IngredientId, decimal Price);
