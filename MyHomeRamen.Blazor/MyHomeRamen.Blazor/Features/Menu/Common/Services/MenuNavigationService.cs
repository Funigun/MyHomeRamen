using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public static class Admin
        {
            public const string ProductsIndex = "/admin/menu/products";
            public const string CreateProduct = "/admin/menu/products/create";
            public const string ProductsManagement = "/admin/menu/products-management";
            public const string IngredientsManagement = "/admin/menu/ingredients-management";
            public const string IngredientsIndex = "/admin/menu/ingredients";
            public const string CreateIngredient = "/admin/menu/ingredients/create";

            public static string EditProduct(Guid id) => $"/admin/menu/products/{id}/edit";
        }

        public static class Public
        {
            public static string ProductDetail(Guid id) => $"/menu/products/{id}";
        }
    }

    public void ToAdminProductsIndex() => navigation.NavigateTo(Routes.Admin.ProductsIndex);

    public void ToCreateProduct() => navigation.NavigateTo(Routes.Admin.CreateProduct);

    public void ToProductsManagement() => navigation.NavigateTo(Routes.Admin.ProductsManagement);

    public void ToIngredientsManagement() => navigation.NavigateTo(Routes.Admin.IngredientsManagement);

    public void ToAdminIngredientsIndex() => navigation.NavigateTo(Routes.Admin.IngredientsIndex);

    public void ToCreateIngredient() => navigation.NavigateTo(Routes.Admin.CreateIngredient);

    public void ToEditProduct(Guid id) => navigation.NavigateTo(Routes.Admin.EditProduct(id));

    public void ToProductDetail(Guid id) => navigation.NavigateTo(Routes.Public.ProductDetail(id));
}
