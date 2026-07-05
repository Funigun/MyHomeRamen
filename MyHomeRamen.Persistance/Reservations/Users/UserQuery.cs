using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IUserQuery
{
    async Task<bool> IUserQuery.ExistsAsync(UserId userId, CancellationToken cancellationToken)
        => await Set<User>().AsNoTracking().AnyAsync(user => user.Id == userId, cancellationToken);
}
