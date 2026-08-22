using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Orders.ExternalApi;

public sealed class OrdersPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "Orders";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
