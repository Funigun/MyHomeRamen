using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;

namespace MyHomeRamen.Persistance.Payments;

public partial class PaymentsDbContext : IPaymentChannelRepository
{
    public void Add(PaymentChannel entity) => PaymentChannels.Add(entity);

    public void AddRange(IEnumerable<PaymentChannel> entities) => PaymentChannels.AddRange(entities);

    public async Task<bool> Exists(Expression<Func<PaymentChannel, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentChannels.AnyAsync(predicate, cancellationToken);

    public void Delete(PaymentChannel entity) => PaymentChannels.Remove(entity);

    public async Task<int> ExecuteDelete(Expression<Func<PaymentChannel, bool>> predicate, CancellationToken cancellationToken)
        => await PaymentChannels.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<int> ExecuteUpdate(Expression<Func<PaymentChannel, bool>> filterPredicate, Dictionary<Expression<Func<PaymentChannel, object>>, Expression> valuesToUpdate, CancellationToken cancellationToken)
    {
        UpdateSettersBuilder<PaymentChannel>? settersBuilder = PrepareSettersBuilder(valuesToUpdate);
        return await PaymentChannels.Where(filterPredicate).ExecuteUpdateAsync(s => settersBuilder.BuildSettersExpression(), cancellationToken);
    }

    async Task<int> IRepository<PaymentChannel, PaymentChannelId>.Count(CancellationToken cancellationToken)
        => await PaymentChannels.CountAsync(cancellationToken);

    IPaymentChannelQuery IPaymentChannelRepository.Query() => this;

    IPaymentChannelSpecification IPaymentChannelRepository.Specification() => this;
}
