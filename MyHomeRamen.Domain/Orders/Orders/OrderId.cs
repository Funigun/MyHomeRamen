using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Orders.Orders;

public readonly record struct OrderId(Guid Value) : IEntityId
{
    public static implicit operator Guid(OrderId id) => id.Value;

    public static implicit operator OrderId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
