using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Roles;
using MyHomeRamen.Features.Payments.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IRoleQuery
{
    public async Task<Role?> GetByNameWithPermissionsAsync(string name, CancellationToken cancellationToken)
        => await Roles.AsNoTracking()
                      .Include(role => role.Permissions)
                      .FirstOrDefaultAsync(role => role.Name == name, cancellationToken);
}
