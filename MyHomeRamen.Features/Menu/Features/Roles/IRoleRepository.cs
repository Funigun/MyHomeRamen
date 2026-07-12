using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Roles;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
