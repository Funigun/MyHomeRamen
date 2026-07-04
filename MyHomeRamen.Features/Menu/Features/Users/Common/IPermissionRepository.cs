using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>, IPermissionQuery, IPermissionSpecification
{
    IPermissionQuery Query();

    IPermissionSpecification Specification();
}
