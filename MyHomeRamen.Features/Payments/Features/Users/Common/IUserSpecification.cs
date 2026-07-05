using MyHomeRamen.Domain.Payments.Users;

namespace MyHomeRamen.Features.Payments.Features.Users.Common;

public interface IUserSpecification
{
    Task<User> ByIdAsync(UserId userId, CancellationToken cancellationToken);
}
