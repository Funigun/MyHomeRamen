using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Basket> baskets)
    {
        public IQueryable<Basket> ForUser(UserId userId)
            => baskets
                .AsNoTracking()
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.BaseIngredients)
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.CustomIngredients)
                .Where(b => b.User.Id == userId && b.Status == BasketStatus.Active);

        public IQueryable<Basket> ForUserTracked(UserId userId)
            => baskets
                .Where(b => b.User.Id == userId && b.Status == BasketStatus.Active);
    }

    public static IQueryable<Basket> GetCurrentBasketSummary(this IQueryable<Basket> baskets, Guid userId)
        => baskets.AsNoTracking()
                  .Where(b => b.User.Id == (UserId)userId && b.Status == BasketStatus.Active)
                  .Include(basket => basket.Items)
                    .ThenInclude(item => item.Product);
}
