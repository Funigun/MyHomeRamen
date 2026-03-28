using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Orders.Orders;

namespace MyHomeRamen.Domain.Orders.Events;

public sealed class OrderAcceptedEvent(Order order) : IDomainEvent
{
    public Order Order { get; } = order;
}
