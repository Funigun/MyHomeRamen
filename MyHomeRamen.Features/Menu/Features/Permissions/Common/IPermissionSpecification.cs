using MyHomeRamen.Domain.Menu.Permssions;

namespace MyHomeRamen.Features.Menu.Features.Permissions.Common;

public interface IPermissionSpecification
{
    Task<Permission> ById(PermissionId permissionId, CancellationToken cancellationToken);
}
