using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IRoleSpecification
{
    public async Task<Role> ByIdAsync(RoleId roleId, CancellationToken cancellationToken)
        => await Set<Role>().FirstAsync(role => role.Id == roleId, cancellationToken);
}
