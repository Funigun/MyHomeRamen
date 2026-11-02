namespace MyHomeRamen.Features.Identity.Permissions;

public static class MenuPermissionConstants
{
    public const string CanManageFavourites = "CanManageFavourites";
    public const string CanManageProducts = "Product.Manage";
    public const string CanAddProduct = "Product.Add";
    public const string CanEditProduct = "Product.Edit";
    public const string CanDeleteProduct = "Product.Delete";
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
    public const string CanViewMenuCard = "MenuCard.View";
    public const string CanAddMenuCard = "MenuCard.Add";
    public const string CanEditMenuCard = "MenuCard.Edit";
    public const string CanDeleteMenuCard = "MenuCard.Delete";
    public const string CanManageMenuCards = "MenuCard.Manage";
    public const string CanViewOffer = "Offer.View";
    public const string CanAddOffer = "Offer.Add";
    public const string CanEditOffer = "Offer.Edit";
    public const string CanDeleteOffer = "Offer.Delete";
    public const string CanManageOffers = "Offer.Manage";
    public const string CanViewIngredientPrice = "IngredientPrice.View";
    public const string CanAddIngredientPrice = "IngredientPrice.Add";
    public const string CanEditIngredientPrice = "IngredientPrice.Edit";
    public const string CanDeleteIngredientPrice = "IngredientPrice.Delete";
    public const string CanManageIngredientPrices = "IngredientPrice.Manage";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CanManageFavourites, CanManageProducts, CanAddProduct, CanEditProduct, CanDeleteProduct,
        CanManageCategories, CanAddCategory, CanEditCategory, CanDeleteCategory,
        CanManageIngredients, CanAddIngredient, CanEditIngredient, CanDeleteIngredient, CanManageMenu,
        CanCreateMenu, CanEditMenu, CanDeleteMenu,
        CanViewMenuCard, CanAddMenuCard, CanEditMenuCard, CanDeleteMenuCard, CanManageMenuCards,
        CanViewOffer, CanAddOffer, CanEditOffer, CanDeleteOffer, CanManageOffers,
        CanViewIngredientPrice, CanAddIngredientPrice, CanEditIngredientPrice, CanDeleteIngredientPrice,
        CanManageIngredientPrices
    ];

    public static IEnumerable<string> CustomerViewPermissions =>
    [
        CanViewMenuCard, CanViewOffer, CanViewIngredientPrice
    ];

    public static IEnumerable<string> RestaurantScopedPermissions =>
    [
        CanViewMenuCard, CanAddMenuCard, CanEditMenuCard, CanDeleteMenuCard, CanManageMenuCards,
        CanViewOffer, CanAddOffer, CanEditOffer, CanDeleteOffer, CanManageOffers,
        CanViewIngredientPrice, CanAddIngredientPrice, CanEditIngredientPrice, CanDeleteIngredientPrice,
        CanManageIngredientPrices
    ];
}
