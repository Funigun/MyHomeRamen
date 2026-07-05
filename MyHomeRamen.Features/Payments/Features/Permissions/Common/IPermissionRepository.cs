using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Permissions.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>, IPermissionQuery, IPermissionSpecification
{
    IPermissionQuery Query();

    IPermissionSpecification Specification();
}
