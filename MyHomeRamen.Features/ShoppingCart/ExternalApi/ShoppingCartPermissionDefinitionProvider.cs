using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.ShoppingCart.ExternalApi;

public sealed class ShoppingCartPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "ShoppingCart";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
