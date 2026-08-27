using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        public bool CanManageProducts() => currentUser.Permissions.Contains(PermissionConstants.CanManageProducts);
        public bool CanAddProduct() => currentUser.Permissions.Contains(PermissionConstants.CanAddProduct);
        public bool CanEditProduct() => currentUser.Permissions.Contains(PermissionConstants.CanEditProduct);
        public bool CanDeleteProduct() => currentUser.Permissions.Contains(PermissionConstants.CanDeleteProduct);

        public bool CanManageCategories() => currentUser.Permissions.Contains(PermissionConstants.CanManageCategories);
        public bool CanAddCategory() => currentUser.Permissions.Contains(PermissionConstants.CanAddCategory);
        public bool CanEditCategory() => currentUser.Permissions.Contains(PermissionConstants.CanEditCategory);
        public bool CanDeleteCategory() => currentUser.Permissions.Contains(PermissionConstants.CanDeleteCategory);

        public bool CanManageIngredients() => currentUser.Permissions.Contains(PermissionConstants.CanManageIngredients);
        public bool CanAddIngredient() => currentUser.Permissions.Contains(PermissionConstants.CanAddIngredient);
        public bool CanEditIngredient() => currentUser.Permissions.Contains(PermissionConstants.CanEditIngredient);
        public bool CanDeleteIngredient() => currentUser.Permissions.Contains(PermissionConstants.CanDeleteIngredient);

        public bool CanManageMenu() => currentUser.Permissions.Contains(PermissionConstants.CanManageMenu);
        public bool CanCreateMenu() => currentUser.Permissions.Contains(PermissionConstants.CanCreateMenu);
        public bool CanEditMenu() => currentUser.Permissions.Contains(PermissionConstants.CanEditMenu);
        public bool CanDeleteMenu() => currentUser.Permissions.Contains(PermissionConstants.CanDeleteMenu);
    }
}
