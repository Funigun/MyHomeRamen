using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Identity.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<IReadOnlyCollection<Permission>> All(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PermissionGrant>> ByUserId(Guid userId, CancellationToken cancellationToken);
}
