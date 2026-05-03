using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class ShoppingCartDbContextExtensions
{
    extension(ShoppingCartDbContext dbContext)
    {
        public async Task<UserId> GetUserId(bool getGuest, CancellationToken cancellationToken)
        {
            return await dbContext.Users.AsNoTracking()
                                        .Where(user => user.IsGuest == getGuest)
                                        .Select(user => user.Id)
                                        .FirstAsync(cancellationToken);
        }
    }
}
