namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

public static class RestaurantRoutes
{
    public const string BrowseRestaurants = "/restaurants/browse";
    public const string SelectRestaurant = "/admin/select-restaurant";
    public const string Index = "/restaurants/companies/restaurants";
    public const string Create = Index + "/create";
    public const string EditTemplate = Index + "/{Id:guid}/edit";

    public static string Edit(Guid id) => $"{Index}/{id}/edit";
}
