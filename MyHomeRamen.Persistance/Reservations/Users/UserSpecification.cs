using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class ReservationsDbContext : IUserSpecification
{
    async Task<User> IUserSpecification.ByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await Set<User>().FirstAsync(user => user.Id == userId, cancellationToken);
}
