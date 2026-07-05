using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Features.Reservations.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default);
}
