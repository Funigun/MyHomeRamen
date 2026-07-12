using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Features.Identity.Features.Roles.Common;

public interface IRoleSpecification
{
    Task<Role> ByName(string roleName, CancellationToken cancellationToken);
}
