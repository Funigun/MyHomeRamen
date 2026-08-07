using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Reservations.Features.Users.Common;

namespace MyHomeRamen.Persistance.Reservations;

public partial class UserRepository : IUserSpecification
{
    async Task<User> IUserSpecification.ByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await reservationsDbContext.Users.FirstAsync(user => user.Id == userId, cancellationToken);
}
