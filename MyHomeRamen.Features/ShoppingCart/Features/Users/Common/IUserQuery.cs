using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

public interface IUserQuery
{
    Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken);

    Task<UserId> GetUserIdAsync(bool isGuest, CancellationToken cancellationToken);
}
