using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Baskets.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketRepository
{
    public void Add(Basket entity) => ShoppingCarts.Add(entity);

    public void AddRange(IEnumerable<Basket> entities) => ShoppingCarts.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Basket, bool>> predicate, CancellationToken cancellationToken)
        => await ShoppingCarts.AnyAsync(predicate, cancellationToken);

    public void Delete(Basket entity) => ShoppingCarts.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Basket, bool>> predicate, CancellationToken cancellationToken)
        => await ShoppingCarts.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Basket, bool>> filterPredicate, Dictionary<Expression<Func<Basket, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Basket>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await ShoppingCarts.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Basket, BasketId>.Count(CancellationToken cancellationToken)
        => await ShoppingCarts.CountAsync(cancellationToken);

    IBasketQuery IBasketRepository.Query() => this;

    IBasketSpecification IBasketRepository.Specification() => this;
}
