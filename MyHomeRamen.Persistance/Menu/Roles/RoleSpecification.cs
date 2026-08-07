using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Features.Menu.Features.Roles;

namespace MyHomeRamen.Persistance.Menu;

public partial class RoleRepository : IRoleSpecification
{
    public async Task<Role> ById(RoleId roleId, CancellationToken cancellationToken)
        => await menuDbContext.Roles.Include(role => role.Permissions)
                                    .FirstAsync(role => role.Id == roleId, cancellationToken);

    public async Task<Role?> ByName(string name, CancellationToken cancellationToken)
        => await menuDbContext.Roles.Include(role => role.Permissions)
                                    .FirstOrDefaultAsync(role => role.Name == name, cancellationToken);

    public async Task<List<Role>> GetAllWithPermissions(CancellationToken cancellationToken)
        => await menuDbContext.Roles.Include(role => role.Permissions)
                                    .ToListAsync(cancellationToken);
}
