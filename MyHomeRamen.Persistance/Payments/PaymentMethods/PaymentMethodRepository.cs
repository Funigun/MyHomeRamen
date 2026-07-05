using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentMethodRepository
{
    public void Add(PaymentMethod entity) => PaymentMethods.Add(entity);

    public void AddRange(IEnumerable<PaymentMethod> entities) => PaymentMethods.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<PaymentMethod, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentMethods.AnyAsync(predicate, cancellationToken);

    public void Delete(PaymentMethod entity) => PaymentMethods.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<PaymentMethod, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentMethods.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<PaymentMethod, bool>> filterPredicate, Dictionary<Expression<Func<PaymentMethod, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<PaymentMethod>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await PaymentMethods.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<PaymentMethod, PaymentMethodId>.Count(CancellationToken cancellationToken)
        => await PaymentMethods.CountAsync(cancellationToken);

    IPaymentMethodQuery IPaymentMethodRepository.Query() => this;

    IPaymentMethodSpecification IPaymentMethodRepository.Specification() => this;
}
