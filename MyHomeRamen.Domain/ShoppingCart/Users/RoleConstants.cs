namespace MyHomeRamen.Domain.ShoppingCart.Users;

public static class RoleConstants
{
    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles =>
     [
        Customer
     ];
}
