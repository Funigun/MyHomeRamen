using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class BasktetItemRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<BasketItem, BasketItemId>(shoppingCartDbContext, cacheService), IBasketItemRepository
{
    IBasketItemQuery IBasketItemRepository.Query() => this;

    IBasketItemLoader IBasketItemRepository.Load() => this;
}
