using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Orders.Payments;

public readonly record struct PaymentId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentId id) => id.Value;
    
    public static implicit operator PaymentId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
