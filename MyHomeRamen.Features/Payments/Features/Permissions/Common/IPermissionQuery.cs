using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken = default);
}
