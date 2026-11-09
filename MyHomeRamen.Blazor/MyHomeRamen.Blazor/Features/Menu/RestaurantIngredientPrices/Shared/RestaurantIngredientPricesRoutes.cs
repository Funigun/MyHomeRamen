namespace MyHomeRamen.Blazor.Features.Menu.RestaurantIngredientPrices.Shared;

public static class RestaurantIngredientPricesRoutes
{
    private const string RestaurantMenuRoot = "/admin/restaurants";
    public const string IndexTemplate = RestaurantMenuRoot + "/{RestaurantId:guid}/menu/ingredient-prices";

    public static string Index(Guid restaurantId) => $"{RestaurantMenuRoot}/{restaurantId}/menu/ingredient-prices";
}
