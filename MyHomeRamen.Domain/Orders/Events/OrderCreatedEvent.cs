using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Orders.Orders;

namespace MyHomeRamen.Domain.Orders.Events;

public sealed class OrderCreatedEvent(Order order) : IDomainEvent
{
    public Order Order { get; } = order;
}
