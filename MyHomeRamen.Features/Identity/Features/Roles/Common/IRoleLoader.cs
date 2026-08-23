using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Features.Identity.Features.Roles.Common;

public interface IRoleLoader
{
    Task<Role?> ByName(string roleName, CancellationToken cancellationToken);
}
