using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

public interface IBasketItemRepository : IRepository<BasketItem, BasketItemId>
{
    IBasketItemQuery Query();

    IBasketItemLoader Load();
}
