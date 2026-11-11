namespace MyHomeRamen.Blazor.Features.Menu.MenuCards.Shared;

public static class MenuCardRoutes
{
    private const string RestaurantMenuRoot = "/admin/restaurants";
    public const string IndexTemplate = RestaurantMenuRoot + "/{RestaurantId:guid}/menu/cards";
    public const string CreateTemplate = IndexTemplate + "/create";
    public const string ViewTemplate = IndexTemplate + "/{MenuCardId:guid}";
    public const string EditTemplate = ViewTemplate + "/edit";

    public static string Index(Guid restaurantId) => $"{RestaurantMenuRoot}/{restaurantId}/menu/cards";
    public static string Create(Guid restaurantId) => $"{Index(restaurantId)}/create";
    public static string View(Guid restaurantId, Guid menuCardId) => $"{Index(restaurantId)}/{menuCardId}";
    public static string Edit(Guid restaurantId, Guid menuCardId) => $"{View(restaurantId, menuCardId)}/edit";
}
