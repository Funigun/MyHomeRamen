using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        public bool CanViewBasket() => currentUser.Permissions.Contains(PermissionConstants.CanViewBasket);
        public bool CanAddProduct() => currentUser.Permissions.Contains(PermissionConstants.CanAddProduct);
        public bool CanUpdateProductQuantity() => currentUser.Permissions.Contains(PermissionConstants.CanUpdateProductQuantity);
        public bool CanRemoveProduct() => currentUser.Permissions.Contains(PermissionConstants.CanRemoveProduct);
        public bool CanCheckout() => currentUser.Permissions.Contains(PermissionConstants.CanCheckout);
    }
}
