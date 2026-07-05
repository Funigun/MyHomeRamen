using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Features.Reservations.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken);
}
