namespace MyHomeRamen.Domain.Menu.Users;

public static class PermissionConstants
{
    public const string CanManageFavourites = "CanManageFavourites";

    public const string CanManageProducts = "Product.Manage";
    public const string CanAddProduct = "Product.Add";
    public const string CanEditProduct = "Product.Edit";
    public const string CanDeleteProduct = "Product.Delete";
    public const string CanEditProductsRecipes = "Product.EditRecipes";

    public const string CanManageCategories = "Category.Manage";
    public const string CanAddCategory = "Category.Add";
    public const string CanEditCategory = "Category.Edit";
    public const string CanDeleteCategory = "Category.Delete";

    public const string CanManageIngredients = "Ingredient.Manage";
    public const string CanAddIngredient = "Ingredient.Add";
    public const string CanEditIngredient = "Ingredient.Edit";
    public const string CanDeleteIngredient = "Ingredient.Delete";

    public const string CanManageMenu = "Menu.Manage";
    public const string CanCreateMenu = "Menu.Create";
    public const string CanEditMenu = "Menu.Edit";
    public const string CanDeleteMenu = "Menu.Delete";

    public static IEnumerable<string> AvailablePermissions =>
     [
        CanManageFavourites,
        CanManageProducts,
        CanAddProduct,
        CanEditProduct,
        CanDeleteProduct,
        CanEditProductsRecipes,
        CanManageCategories,
        CanAddCategory,
        CanEditCategory,
        CanDeleteCategory,
        CanManageIngredients,
        CanAddIngredient,
        CanEditIngredient,
        CanDeleteIngredient,
        CanManageMenu,
        CanCreateMenu,
        CanEditMenu,
        CanDeleteMenu
     ];
}
