using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Features.Reservations.Features.Users.Common;

public interface IUserQuery
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken);
}
