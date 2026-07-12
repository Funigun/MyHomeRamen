using MyHomeRamen.Domain.Menu.Roles;

namespace MyHomeRamen.Features.Menu.Features.Roles;

public interface IRoleSpecification
{
    Task<Role> ById(RoleId roleId, CancellationToken cancellationToken);

    Task<Role?> ByName(string menuRoleName, CancellationToken cancellationToken);

    Task<List<Role>> GetAllWithPermissions(CancellationToken cancellationToken);
}
