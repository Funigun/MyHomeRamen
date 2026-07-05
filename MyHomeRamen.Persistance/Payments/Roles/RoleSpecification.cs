using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Payments.Features.Roles.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IRoleSpecification
{
    public async Task<Role?> ByIdAsync(RoleId id, CancellationToken cancellationToken)
        => await Roles.AsNoTracking().FirstOrDefaultAsync(role => role.Id == id, cancellationToken);
}
