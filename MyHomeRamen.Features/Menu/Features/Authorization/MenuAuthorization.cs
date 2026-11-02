using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Authorization;

internal static class MenuAuthorization
{
    public static bool CanManageRestaurant(
        ICurrentUser currentUser,
        Guid? restaurantId,
        string managePermission,
        string operationPermission)
        => restaurantId.HasValue
            && currentUser.HasPermission("Menu", managePermission, restaurantId)
            && currentUser.HasPermission("Menu", operationPermission, restaurantId);
}
