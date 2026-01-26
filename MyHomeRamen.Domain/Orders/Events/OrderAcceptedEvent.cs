using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders.Events;

public sealed class OrderAcceptedEvent(Order order) : IDomainEvent
{
    public Order Order { get; } = order;
}
