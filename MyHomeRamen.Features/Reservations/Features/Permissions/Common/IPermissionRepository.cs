using MyHomeRamen.Domain.Reservations.Permissions;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Reservations.Features.Permissions.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>
{
    IPermissionQuery Query();

    IPermissionSpecification Specification();
}
