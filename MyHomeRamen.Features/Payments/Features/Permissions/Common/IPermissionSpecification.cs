using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken);
}
