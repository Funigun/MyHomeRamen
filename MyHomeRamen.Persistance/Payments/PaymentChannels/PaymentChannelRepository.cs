using MyHomeRamen.Domain.Payments.PaymentChannels;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.PaymentChannels.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class PaymentChannelRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<PaymentChannel, PaymentChannelId>(paymentsDbContext, cacheService), IPaymentChannelRepository
{
    public IPaymentChannelQuery Query() => this;

    public IPaymentChannelLoader Load() => this;
}