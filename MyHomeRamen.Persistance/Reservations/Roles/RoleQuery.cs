using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IRoleQuery
{
    public async Task<Role?> GetByNameWithPermissionsAsync(string roleName, CancellationToken cancellationToken = default)
        => await Set<Role>().AsNoTracking().Include(role => role.Permissions).FirstOrDefaultAsync(role => role.Name == roleName, cancellationToken);
}
