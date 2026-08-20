using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Identity.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleLoader Load();
}
