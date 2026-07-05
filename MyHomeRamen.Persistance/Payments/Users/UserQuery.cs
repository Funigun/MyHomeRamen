using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Payments.Features.Users.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IUserQuery
{
    public async Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)
        => await Users.AsNoTracking().AnyAsync(user => user.Id == userId, cancellationToken);
}
