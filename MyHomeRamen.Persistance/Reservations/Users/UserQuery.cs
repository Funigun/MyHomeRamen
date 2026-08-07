using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class UserRepository : IUserQuery
{
    async Task<bool> IUserQuery.ExistsAsync(UserId userId, CancellationToken cancellationToken)
        => await reservationsDbContext.Users.AsNoTracking().AnyAsync(user => user.Id == userId, cancellationToken);
}
