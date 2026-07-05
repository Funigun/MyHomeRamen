using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;

public interface IPermissionRepository : IRepository<Permission, PermissionId>
{
    IPermissionQuery Query();

    IPermissionSpecification Specification();
}
