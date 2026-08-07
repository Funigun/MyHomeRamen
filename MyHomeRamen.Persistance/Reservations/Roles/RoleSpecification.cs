using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Roles;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class RoleRepository : IRoleSpecification
{
    public async Task<Role> ByIdAsync(RoleId roleId, CancellationToken cancellationToken)
        => await reservationsDbContext.Roles.FirstAsync(role => role.Id == roleId, cancellationToken);
}
