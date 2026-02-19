namespace MyHomeRamen.Domain.ShoppingCart.Users;

public static class PermissionConstants
{
    public const string CanViewBasket = "CanViewBasket";
    public const string CanAddProduct = "CanAddProduct";
    public const string CanUpdateProductQuantity = "CanUpdateProductQuantity";
    public const string CanRemoveProduct = "CanRemoveProduct";
    public const string CanCheckout = "CanCheckout";

    public static IEnumerable<string> AvailablePermissions =>
     [
        CanViewBasket,
        CanAddProduct,
        CanUpdateProductQuantity,
        CanRemoveProduct,
        CanCheckout
     ];
}
