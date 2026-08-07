using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PermissionRepository : IPermissionQuery
{
    public async Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken)
        => await paymentsDbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == id, cancellationToken);
}
