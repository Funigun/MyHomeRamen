using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

public interface IBasketRepository : IRepository<Basket, BasketId>
{
    IBasketQuery Query();

    IBasketSpecification Specification();
}
