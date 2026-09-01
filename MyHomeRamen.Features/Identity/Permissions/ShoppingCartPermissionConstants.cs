namespace MyHomeRamen.Features.Identity.Permissions;

public static class ShoppingCartPermissionConstants
{
    public const string CanViewBasket = "CanViewBasket";
    public const string CanAddProduct = "CanAddProduct";
    public const string CanUpdateProductQuantity = "CanUpdateProductQuantity";
    public const string CanRemoveProduct = "CanRemoveProduct";
    public const string CanCheckout = "CanCheckout";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CanViewBasket, CanAddProduct, CanUpdateProductQuantity, CanRemoveProduct, CanCheckout
    ];
}
