using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Identity.ExternalApi;

internal class IdentityPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName { get; } = "Identity";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
