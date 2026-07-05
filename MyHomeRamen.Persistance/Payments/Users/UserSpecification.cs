using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Payments.Features.Users.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IUserSpecification
{
    public async Task<User> ByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await Users.Include(user => user.Roles)
                      .Include(user => user.Permissions)
                      .AsSplitQuery()
                      .FirstAsync(user => user.Id == userId, cancellationToken);
}
