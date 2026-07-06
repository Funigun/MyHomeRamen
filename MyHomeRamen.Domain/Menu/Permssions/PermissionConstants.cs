namespace MyHomeRamen.Domain.Menu.Permssions;

public static class PermissionConstants
{
    public const string CanManageFavourites = "CanManageFavourites";

    public const string CanViewProductsManagementView = "CanViewProductsManagementView";
    public const string CanAddProduct = "CanAddProduct";
    public const string CanEditProduct = "CanEditProduct";
    public const string CanDeleteProduct = "CanDeleteProduct";
    public const string CanEditProductsRecipes = "CanEditProductsRecipes";

    public const string CanViewCategoriesManagementView = "CanViewCategoriesManagementView";
    public const string CanAddCategory = "CanAddCategory";
    public const string CanEditCategory = "CanEditCategory";
    public const string CanDeleteCategory = "CanDeleteCategory";

    public const string CanViewIngredientsManagementView = "CanViewIngredientsManagementView";
    public const string CanAddIngredient = "CanAddIngredient";
    public const string CanEditIngredient = "CanEditIngredient";
    public const string CanDeleteIngredient = "CanDeleteIngredient";

    public static IEnumerable<string> AvailablePermissions =>
     [
        CanManageFavourites,
        CanViewProductsManagementView,
        CanAddProduct,
        CanEditProduct,
        CanDeleteProduct,
        CanEditProductsRecipes,
        CanViewCategoriesManagementView,
        CanAddCategory,
        CanEditCategory,
        CanDeleteCategory,
        CanViewIngredientsManagementView,
        CanAddIngredient,
        CanEditIngredient,
        CanDeleteIngredient
     ];
}
