using MyHomeRamen.Domain.Payments.PaymentGateways;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.PaymentGateways.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class PaymentGatewayRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<PaymentGateway, PaymentGatewayId>(paymentsDbContext, cacheService), IPaymentGatewayRepository
{
    public IPaymentGatewayQuery Query() => this;

    public IPaymentGatewayLoader Load() => this;
}