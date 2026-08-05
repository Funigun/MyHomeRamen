using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class BasketRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService): BaseRepository<Basket, BasketId>(shoppingCartDbContext, cacheService), IBasketRepository
{
    IBasketQuery IBasketRepository.Query() => this;

    IBasketSpecification IBasketRepository.Specification() => this;
}
