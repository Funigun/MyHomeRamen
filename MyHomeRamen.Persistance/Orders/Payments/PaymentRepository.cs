using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Orders.Features.Payments.Common;

namespace MyHomeRamen.Persistance.Orders;

public partial class OrdersDbContext : IPaymentRepository
{
    public void Add(Payment entity) => Payments.Add(entity);

    public void AddRange(IEnumerable<Payment> entities) => Payments.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken)
        => await Payments.AnyAsync(predicate, cancellationToken);

    public void Delete(Payment entity) => Payments.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken)
        => await Payments.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<Payment, bool>> filterPredicate, Dictionary<Expression<Func<Payment, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<Payment>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await Payments.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<Payment, PaymentId>.Count(CancellationToken cancellationToken)
        => await Payments.CountAsync(cancellationToken);

    IPaymentQuery IPaymentRepository.Query() => this;

    IPaymentSpecification IPaymentRepository.Specification() => this;
}