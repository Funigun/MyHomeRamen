using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.ExternalApi;

public sealed class MenuPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "Menu";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
