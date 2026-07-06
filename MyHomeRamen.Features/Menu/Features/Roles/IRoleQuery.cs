using MyHomeRamen.Domain.Menu.Roles;

namespace MyHomeRamen.Features.Menu.Features.Roles;

public interface IRoleQuery
{
    Task<bool> Exists(RoleId roleId, CancellationToken cancellationToken);
}
