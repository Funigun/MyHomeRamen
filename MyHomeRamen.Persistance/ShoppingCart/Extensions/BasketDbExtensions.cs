using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
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

        public IQueryable<Basket> GetByIdForUser(BasketId basketId, UserId userId)
            => baskets
                .AsNoTracking()
                .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active);

        public IQueryable<Basket> GetByIdForUserTracked(BasketId basketId, UserId userId)
            => baskets
                .Include(b => b.Items)
                .Where(b => b.Id == basketId && b.User.Id == userId && b.Status == BasketStatus.Active);

        public IQueryable<Basket> GetCurrentBasketSummary(Guid userId)
            => baskets.AsNoTracking()
                      .Where(b => b.User.Id == (UserId)userId && b.Status == BasketStatus.Active)
                      .Include(basket => basket.Items)
                        .ThenInclude(item => item.Product);

        public Task<bool> ItemExistsQuery(UserId userId, BasketItemId basketItemId, BasketId basketId, CancellationToken cancellationToken)
            => baskets.AsNoTracking()
                      .AnyAsync(
                                b => b.Id == basketId
                             && b.User.Id == userId
                             && b.Items.Any(i => i.Id == basketItemId),
                                cancellationToken);
    }
}
