using MyHomeRamen.Domain.Orders.Payments;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Payments.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class PaymentRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Payment, PaymentId>(ordersDbContext, cacheService), IPaymentRepository
{
    public IPaymentQuery Query() => this;

    public IPaymentSpecification Specification() => this;
}