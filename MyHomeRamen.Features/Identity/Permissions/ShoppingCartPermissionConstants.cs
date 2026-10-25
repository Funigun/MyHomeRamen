namespace MyHomeRamen.Features.Identity.Permissions;

public static class ShoppingCartPermissionConstants
{
    public const string ManageBasket = "Basket.Manage";

    public static IEnumerable<string> AvailablePermissions =>
    [
        ManageBasket
    ];

    public static IEnumerable<string> GuestPermissions =>
    [
        ManageBasket
    ];
}
