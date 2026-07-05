using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Reservations.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
