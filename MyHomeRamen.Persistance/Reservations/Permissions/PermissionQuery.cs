using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IPermissionQuery
{
    public async Task<Permission?> ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default)
        => await Set<Permission>().AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == permissionId, cancellationToken);
}
