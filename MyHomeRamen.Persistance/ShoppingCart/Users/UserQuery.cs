using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IUserQuery
{
    public async Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public async Task<UserId> GetUserIdAsync(bool isGuest, CancellationToken cancellationToken)
        => await Users.AsNoTracking()
            .Where(user => user.IsGuest == isGuest)
            .Select(user => user.Id)
            .FirstAsync(cancellationToken);
}
