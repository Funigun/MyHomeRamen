using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class ShoppingCartDbContextExtensions
{
    extension(ShoppingCartDbContext dbContext)
    {
        public async Task<UserId> GetUserId(bool getGuest, CancellationToken cancellationToken)
        {
            return await dbContext.User.Query().GetUserIdAsync(getGuest, cancellationToken);
        }
    }
}
