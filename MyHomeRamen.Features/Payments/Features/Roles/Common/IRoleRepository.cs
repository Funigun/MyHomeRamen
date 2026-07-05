using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Roles.Common;

public interface IRoleRepository : IRepository<Role, RoleId>, IRoleQuery, IRoleSpecification
{
    IRoleQuery Query();

    IRoleSpecification Specification();
}
