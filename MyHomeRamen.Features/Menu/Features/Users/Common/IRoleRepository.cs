using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IRoleRepository : IRepository<Role, RoleId>, IRoleQuery, IRoleSpecification
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
