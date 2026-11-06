namespace MyHomeRamen.Blazor.Features.Menu.RestaurantMenu.Shared;

public static class RestaurantMenuRoutes
{
    public const string CustomerMenu = "/menu";
    public const string CompanyOwnerPreviewTemplate = "/company-owner/restaurants/{RestaurantId:guid}/menu";

    public static string CompanyOwnerPreview(Guid restaurantId)
    {
        if (restaurantId == Guid.Empty)
        {
            throw new ArgumentException("Restaurant identifier must not be empty.", nameof(restaurantId));
        }

        return $"/company-owner/restaurants/{restaurantId}/menu";
    }
}
