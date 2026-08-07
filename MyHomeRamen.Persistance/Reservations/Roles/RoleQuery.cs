using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Roles;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class RoleRepository : IRoleQuery
{
    public async Task<Role?> GetByNameWithPermissionsAsync(string roleName, CancellationToken cancellationToken)
        => await reservationsDbContext.Roles.AsNoTracking().Include(role => role.Permissions).FirstOrDefaultAsync(role => role.Name == roleName, cancellationToken);
}
