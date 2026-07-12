using MyHomeRamen.Domain.Reservations.Permissions;

namespace MyHomeRamen.Features.Reservations.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken);
}
