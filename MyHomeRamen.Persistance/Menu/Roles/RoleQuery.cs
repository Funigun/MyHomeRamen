using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Features.Menu.Features.Roles;

namespace MyHomeRamen.Persistance.Menu;

public partial class RoleRepository : IRoleQuery
{
    private IQueryable<Role> RolesQuery => menuDbContext.Roles.AsNoTracking();

    public async Task<IEnumerable<Role>> GetAll(CancellationToken cancellationToken)
        => await RolesQuery.Include(role => role.Permissions).ToListAsync(cancellationToken);
}
