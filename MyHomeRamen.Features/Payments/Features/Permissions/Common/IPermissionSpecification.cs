using MyHomeRamen.Domain.Payments.Permissions;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken);
}
