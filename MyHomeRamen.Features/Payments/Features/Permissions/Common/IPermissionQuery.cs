using MyHomeRamen.Domain.Payments.Permissions;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken);
}
