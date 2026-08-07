using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Roles;
using MyHomeRamen.Features.Payments.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class RoleRepository : IRoleSpecification
{
    public async Task<Role?> ByIdAsync(RoleId id, CancellationToken cancellationToken)
        => await paymentsDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
}
