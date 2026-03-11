namespace MyHomeRamen.Domain.Menu.Users;

public static class RoleConstants
{
    public const string Admin = "MenuAdmin";

    public const string Employee = "Employee";

    public const string Waiter = "Waiter";

    public const string Chef = "Chef";

    public const string Customer = "Customer";

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
