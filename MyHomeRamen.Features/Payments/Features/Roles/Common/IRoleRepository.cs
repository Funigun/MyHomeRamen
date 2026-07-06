using MyHomeRamen.Domain.Payments.Roles;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
