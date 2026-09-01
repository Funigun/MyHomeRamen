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
        .. CreateDefinitions("Restaurants", RestaurantsPermissionConstants.AvailablePermissions)
    ];

    public static IReadOnlySet<(string Module, string Name)> GuestPermissions { get; } = new HashSet<(string Module, string Name)>();

    private static IEnumerable<PermissionDefinition> CreateDefinitions(string moduleName, IEnumerable<string> permissions) 
        => permissions.Select(permission => new PermissionDefinition(moduleName, permission, permission));
}
