using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>
{
    IPermissionQuery Query();

    IPermissionSpecification Specification();
}
