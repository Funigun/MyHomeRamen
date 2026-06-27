using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Payments.PaymentGateways;

public readonly record struct PaymentGatewayId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentGatewayId id) => id.Value;

    public static implicit operator PaymentGatewayId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
