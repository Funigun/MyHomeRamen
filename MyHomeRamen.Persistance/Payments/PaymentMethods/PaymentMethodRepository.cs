using MyHomeRamen.Domain.Payments.PaymentMethods;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.PaymentMethods.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class PaymentMethodRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<PaymentMethod, PaymentMethodId>(paymentsDbContext, cacheService), IPaymentMethodRepository
{
    public IPaymentMethodQuery Query() => this;

    public IPaymentMethodLoader Load() => this;
}