namespace MyHomeRamen.Domain.ShoppingCart.Users;

public static class PermissionConstants
{
    public const string CanViewBasket = "Basket.View";
    public const string CanAddProduct = "Basket.Product.Add";
    public const string CanUpdateProductQuantity = "Basket.Product.UpdateQuantity";
    public const string CanRemoveProduct = "Basket.Product.Remove";
    public const string CanCheckout = "Basket.Checkout";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CanViewBasket,
        CanAddProduct,
        CanUpdateProductQuantity,
        CanRemoveProduct,
        CanCheckout
    ];
}
