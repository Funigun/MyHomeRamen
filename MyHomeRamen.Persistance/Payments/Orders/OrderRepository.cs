using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Payments.Features.Orders.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IOrderRepository
{
    public void Add(Order entity) => Orders.Add(entity);

    public void AddRange(IEnumerable<Order> entities) => Orders.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken)
        => await Orders.AnyAsync(predicate, cancellationToken);

    public void Delete(Order entity) => Orders.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Order, bool>> predicate, CancellationToken cancellationToken)
        => await Orders.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Order, bool>> filterPredicate, Dictionary<Expression<Func<Order, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Order>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Orders.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Order, OrderId>.Count(CancellationToken cancellationToken)
        => await Orders.CountAsync(cancellationToken);

    IOrderQuery IOrderRepository.Query() => this;

    IOrderSpecification IOrderRepository.Specification() => this;
}
