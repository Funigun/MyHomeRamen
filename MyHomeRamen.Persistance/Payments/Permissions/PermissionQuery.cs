using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPermissionQuery
{
    public async Task<Permission?> ByIdAsync(PermissionId id, CancellationToken cancellationToken)
        => await Permissions.AsNoTracking().FirstOrDefaultAsync(permission => permission.Id == id, cancellationToken);
}
