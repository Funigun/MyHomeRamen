using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Orders.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class OrderRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Order, OrderId>(ordersDbContext, cacheService), IOrderRepository
{
    public IOrderQuery Query() => this;

    public IOrderSpecification Specification() => this;
}