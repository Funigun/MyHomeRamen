using MyHomeRamen.Domain.Payments.Orders;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Payments.Features.Orders.Common;

public interface IOrderRepository : IRepository<Order, OrderId>, IOrderQuery, IOrderSpecification
{
    IOrderQuery Query();

    IOrderSpecification Specification();
}
