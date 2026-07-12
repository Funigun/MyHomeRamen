using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

public interface IUserSpecification
{
    Task<User?> ByIdAsync(UserId userId, CancellationToken cancellationToken);
}
