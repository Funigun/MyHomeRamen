using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IRoleQuery
{
    Task<bool> Exists(RoleId roleId, CancellationToken cancellationToken);
}
