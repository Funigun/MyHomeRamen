using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
