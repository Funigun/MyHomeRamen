using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Permissions;
using MyHomeRamen.Features.Reservations.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IPermissionSpecification
{
    async Task<Permission> IPermissionSpecification.ByIdAsync(PermissionId permissionId, CancellationToken cancellationToken)
        => await Permissions.FirstAsync(permission => permission.Id == permissionId, cancellationToken);
}
