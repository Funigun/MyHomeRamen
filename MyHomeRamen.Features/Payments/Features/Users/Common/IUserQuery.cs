using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Users.Common;

public interface IUserQuery
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default);
}
