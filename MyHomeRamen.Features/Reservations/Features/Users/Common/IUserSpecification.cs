using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.Features.Reservations.Features.Users.Common;

public interface IUserSpecification
{
    Task<User> ByIdAsync(UserId userId, CancellationToken cancellationToken = default);
}
