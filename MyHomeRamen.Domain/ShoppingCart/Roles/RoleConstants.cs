using MyHomeRamen.Domain.ShoppingCart.Permissions;

namespace MyHomeRamen.Domain.ShoppingCart.Roles;

public static class RoleConstants
{
    public const string Admin = "ShoppingCartAdmin";

    public const string Employee = "ShoppingCartEmployee";

    public const string Waiter = "ShoppingCartWaiter";

    public const string Chef = "ShoppingCartChef";

    public const string Customer = "ShoppingCartCustomer";

    public static IEnumerable<string> AvailableRoles =>
     [
        Admin,
        Employee,
        Waiter,
        Chef,
        Customer
     ];

    public static Dictionary<string, IEnumerable<string>> DefaultPermissions => new()
    {
        { Admin, PermissionConstants.AvailablePermissions },
        { Employee, PermissionConstants.AvailablePermissions },
        {
            Waiter,
            [PermissionConstants.CanViewBasket,
                  PermissionConstants.CanCheckout]
        },
        {
            Chef,
            [PermissionConstants.CanViewBasket]
        },
        { Customer, PermissionConstants.AvailablePermissions }
    };
}
