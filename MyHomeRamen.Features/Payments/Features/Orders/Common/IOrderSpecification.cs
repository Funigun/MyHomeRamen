using MyHomeRamen.Domain.Payments.Orders;

namespace MyHomeRamen.Features.Payments.Features.Orders.Common;

public interface IOrderSpecification
{
    Task<Order?> ByIdAsync(OrderId id, CancellationToken cancellationToken = default);
}
