using MyHomeRamen.Domain.Menu.Permssions;

namespace MyHomeRamen.Domain.Menu.Roles;

public static class RoleConstants
{
    public const string Admin = "MenuAdmin";

    public const string Employee = "MenuEmployee";

    public const string Waiter = "MenuWaiter";

    public const string Chef = "MenuChef";

    public const string Customer = "MenuCustomer";

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
        { Employee, PermissionConstants.AvailablePermissions.Where(p => p != PermissionConstants.CanManageFavourites) },
        {
            Waiter,
            [PermissionConstants.CanViewProductsManagementView,
                  PermissionConstants.CanViewCategoriesManagementView,
                  PermissionConstants.CanViewIngredientsManagementView]
        },
        {
            Chef,
            [PermissionConstants.CanViewProductsManagementView,
            PermissionConstants.CanEditProduct,
            PermissionConstants.CanEditProductsRecipes,
            PermissionConstants.CanViewCategoriesManagementView,
            PermissionConstants.CanEditCategory,
            PermissionConstants.CanViewIngredientsManagementView,
            PermissionConstants.CanEditIngredient]
        },
        { Customer, [PermissionConstants.CanManageFavourites] }
    };
}
