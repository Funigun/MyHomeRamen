using MyHomeRamen.Domain.Orders.Ingredients;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Ingredients.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class IngredientRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Ingredient, IngredientId>(ordersDbContext, cacheService), IIngredientRepository
{
    public IIngredientQuery Query() => this;

    public IIngredientSpecification Specification() => this;
}