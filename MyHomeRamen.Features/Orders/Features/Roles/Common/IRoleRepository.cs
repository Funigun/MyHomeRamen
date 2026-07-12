using MyHomeRamen.Domain.Orders.Roles;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
