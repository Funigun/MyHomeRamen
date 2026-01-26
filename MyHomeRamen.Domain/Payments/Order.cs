using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments;

public sealed class Order : AuditableEntity, IEntity<OrderId>
{
    public OrderId Id { get; private set; }
}
