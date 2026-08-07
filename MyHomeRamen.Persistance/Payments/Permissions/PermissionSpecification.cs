using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PermissionRepository : IPermissionSpecification
{
    async Task<Permission?> IPermissionSpecification.ByIdAsync(PermissionId id, CancellationToken cancellationToken)
        => await paymentsDbContext.Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == id, cancellationToken);
}
