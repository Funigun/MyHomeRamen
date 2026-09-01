using MyHomeRamen.Domain.Orders.Orders;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Orders.Common;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    IOrderQuery Query();

    IOrderLoader Load();
}
