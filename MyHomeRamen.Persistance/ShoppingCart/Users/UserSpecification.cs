using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class UserRepository : IUserSpecification
{
    public async Task<User?> ByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
}
