using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IPermissionSpecification
{
    Task<Permission> ById(PermissionId permissionId, CancellationToken cancellationToken);
}
