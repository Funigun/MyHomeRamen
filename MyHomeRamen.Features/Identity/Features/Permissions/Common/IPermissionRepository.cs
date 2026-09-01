using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Identity.Features.Permissions.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>
{
    IPermissionQuery Query();

    IPermissionLoader Load();
}
