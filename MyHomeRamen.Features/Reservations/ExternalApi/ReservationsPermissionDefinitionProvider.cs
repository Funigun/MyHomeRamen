using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Reservations.ExternalApi;

public sealed class ReservationsPermissionDefinitionProvider : IPermissionDefinitionProvider
{
    public string ModuleName => "Reservations";

    public IReadOnlyCollection<PermissionDefinition> Permissions => PermissionConstants.AvailablePermissions
        .Select(permission => new PermissionDefinition(permission, permission))
        .ToArray();

    public IReadOnlyCollection<PermissionDefinition> GuestPermissions { get; } = [];
}
