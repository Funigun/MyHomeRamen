using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Contracts;

namespace MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        public bool CanManageBasket() => currentUser.Permissions.Contains(PermissionConstants.ManageBasket);

        public AllowedActionsDto BasketAllowedActions() =>
            new(
                CanCreate: currentUser.CanManageBasket(),
                CanEdit: currentUser.CanManageBasket(),
                CanDelete: false);
    }
}
