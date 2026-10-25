namespace MyHomeRamen.Domain.ShoppingCart.Users;

public static class PermissionConstants
{
    public const string ManageBasket = "Basket.Manage";

    public static IEnumerable<string> AvailablePermissions =>
    [
        ManageBasket
    ];
}
