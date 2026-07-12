using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Features.Menu.Features.Roles;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IRoleSpecification
{
    public async Task<Role> ById(RoleId roleId, CancellationToken cancellationToken)
        => await Roles.Include(role => role.Permissions)
                      .AsSplitQuery()
                      .FirstAsync(role => role.Id == roleId, cancellationToken);

    public async Task<Role?> ByName(string name, CancellationToken cancellationToken)
        => await Roles.Include(role => role.Permissions)
                      .AsSplitQuery()
                      .FirstOrDefaultAsync(role => role.Name == name, cancellationToken);

    public async Task<List<Role>> GetAllWithPermissions(CancellationToken cancellationToken)
        => await Roles.Include(role => role.Permissions)
                      .ToListAsync(cancellationToken);
}
