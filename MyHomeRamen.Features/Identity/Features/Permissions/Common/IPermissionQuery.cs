using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Features.Identity.Features.Permissions.Common;

public interface IPermissionQuery
{
    Task<IReadOnlyCollection<Permission>> All(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Permission>> ByUserId(Guid userId, CancellationToken cancellationToken);
}
