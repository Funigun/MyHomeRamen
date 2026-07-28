using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class PermissionRepository : IPermissionSpecification
{
    public async Task<Permission> ById(PermissionId permissionId, CancellationToken cancellationToken)
        => await First(permission => permission.Id == permissionId, cancellationToken);
}
