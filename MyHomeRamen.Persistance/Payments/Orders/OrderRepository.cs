using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.Orders.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class OrderRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<Order, OrderId>(paymentsDbContext, cacheService), IOrderRepository
{
    public IOrderQuery Query() => this;

    public IOrderLoader Load() => this;
}