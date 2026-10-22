using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Services;

internal static class PermissionCatalog
{
    public static IReadOnlyCollection<PermissionDefinition> Definitions { get; } =
    [
        .. CreateDefinitions("Identity", IdentityPermissionConstants.AvailablePermissions),
        .. CreateDefinitions("Menu", MenuPermissionConstants.AvailablePermissions),
        .. CreateDefinitions("ShoppingCart", ShoppingCartPermissionConstants.AvailablePermissions),
        .. CreateDefinitions("Orders", OrdersPermissionConstants.AvailablePermissions),
        .. CreateDefinitions("Reservations", ReservationsPermissionConstants.AvailablePermissions),
        .. CreateDefinitions("Payments", PaymentsPermissionConstants.AvailablePermissions),
        .. CreateDefinitions(
            "Restaurants",
            RestaurantsPermissionConstants.AvailablePermissions,
            RestaurantsPermissionConstants.ResourceScopedPermissions)
    ];

    public static IReadOnlySet<(string Module, string Name)> GuestPermissions { get; } 
        = ShoppingCartPermissionConstants.GuestPermissions.Select(permission => ("ShoppingCart", permission))
                                                          .ToHashSet();

    public static IReadOnlySet<(string Module, string Name)> CompanyOwnerPermissions { get; }
        = RestaurantsPermissionConstants.CompanyOwnerPermissions.Select(permission => ("Restaurants", permission))
                                                                 .ToHashSet();

    private static IEnumerable<PermissionDefinition> CreateDefinitions(
        string moduleName,
        IEnumerable<string> permissions,
        IEnumerable<string>? resourceScopedPermissions = null)
    {
        HashSet<string> resourceScopedNames = resourceScopedPermissions?.ToHashSet(StringComparer.Ordinal) ?? [];

        return permissions.Select(permission =>
            new PermissionDefinition(moduleName, permission, permission, resourceScopedNames.Contains(permission)));
    }
}
