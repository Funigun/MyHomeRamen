using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentGatewayRepository
{
    public void Add(PaymentGateway entity) => PaymentGateways.Add(entity);

    public void AddRange(IEnumerable<PaymentGateway> entities) => PaymentGateways.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<PaymentGateway, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentGateways.AnyAsync(predicate, cancellationToken);

    public void Delete(PaymentGateway entity) => PaymentGateways.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<PaymentGateway, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentGateways.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<PaymentGateway, bool>> filterPredicate, Dictionary<Expression<Func<PaymentGateway, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<PaymentGateway>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await PaymentGateways.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<PaymentGateway, PaymentGatewayId>.Count(CancellationToken cancellationToken)
        => await PaymentGateways.CountAsync(cancellationToken);

    IPaymentGatewayQuery IPaymentGatewayRepository.Query() => this;

    IPaymentGatewaySpecification IPaymentGatewayRepository.Specification() => this;
}
