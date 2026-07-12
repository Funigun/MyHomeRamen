using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Users.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IUserSpecification
{
    public async Task<User?> ByIdAsync(UserId userId, CancellationToken cancellationToken)
        => await Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
}
