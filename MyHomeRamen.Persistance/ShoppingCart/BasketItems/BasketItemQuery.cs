using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketItemQuery
{
    public async Task<bool> ItemExistsAsync(UserId userId, BasketItemId basketItemId, BasketId basketId, CancellationToken cancellationToken)
        => await ShoppingCarts.AsNoTracking()
            .AnyAsync(b => b.Id == basketId && b.User.Id == userId && b.Items.Any(i => i.Id == basketItemId), cancellationToken);
}
