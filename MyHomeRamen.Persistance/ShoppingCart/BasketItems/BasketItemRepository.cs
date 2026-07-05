using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketItemRepository
{
    public void Add(BasketItem entity) => BasketItems.Add(entity);

    public void AddRange(IEnumerable<BasketItem> entities) => BasketItems.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<BasketItem, bool>> predicate, CancellationToken cancellationToken)
        => await BasketItems.AnyAsync(predicate, cancellationToken);

    public void Delete(BasketItem entity) => BasketItems.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<BasketItem, bool>> predicate, CancellationToken cancellationToken)
        => await BasketItems.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<BasketItem, bool>> filterPredicate, Dictionary<Expression<Func<BasketItem, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<BasketItem>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await BasketItems.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<BasketItem, BasketItemId>.Count(CancellationToken cancellationToken)
        => await BasketItems.CountAsync(cancellationToken);

    IBasketItemQuery IBasketItemRepository.Query() => this;

    IBasketItemSpecification IBasketItemRepository.Specification() => this;
}
