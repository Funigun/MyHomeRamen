using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    public static IQueryable<Basket> GetCurrentBasketSummary(this IQueryable<Basket> baskets, Guid userId)
        => baskets.AsNoTracking()
                  .Where(b => b.User.Id == (UserId)userId && b.Status == BasketStatus.Active)
                  .Include(basket => basket.Items)
                    .ThenInclude(item => item.Product);
}
