using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Features.Menu.Features.Users.Common;

public interface IRoleSpecification
{
    Task<Role> ById(RoleId roleId, CancellationToken cancellationToken);

    Task<Role?> ByName(string menuRoleName, CancellationToken cancellationToken);

    Task<List<Role>> GetAllWithPermissions(CancellationToken cancellationToken = default);
}
