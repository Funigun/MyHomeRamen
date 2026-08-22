using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Payments.ExternalApi;

public sealed class PaymentsPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "Payments";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
