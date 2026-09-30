using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Restaurants.Permissions;

public static class CompanyDetailsPermissions
{
    public static bool CanViewCompanyDetails(this ICurrentUser currentUser)
        => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyView);

    public static bool CanEditCompanyDetails(this ICurrentUser currentUser)
        => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyEdit);
}
