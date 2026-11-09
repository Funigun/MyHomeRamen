namespace MyHomeRamen.Blazor.Features.Menu.Offers.Shared;

public static class OffersRoutes
{
    private const string RestaurantMenuRoot = "/admin/restaurants";
    public const string IndexTemplate = RestaurantMenuRoot + "/{RestaurantId:guid}/menu/offers";
    public const string CreateTemplate = IndexTemplate + "/create";
    public const string EditPriceTemplate = IndexTemplate + "/{OfferId:guid}/price";
    public const string RetireTemplate = IndexTemplate + "/{OfferId:guid}/retire";

    public static string Index(Guid restaurantId) => $"{RestaurantMenuRoot}/{restaurantId}/menu/offers";
    public static string Create(Guid restaurantId) => $"{Index(restaurantId)}/create";
    public static string EditPrice(Guid restaurantId, Guid offerId) => $"{Index(restaurantId)}/{offerId}/price";
    public static string Retire(Guid restaurantId, Guid offerId) => $"{Index(restaurantId)}/{offerId}/retire";
}
