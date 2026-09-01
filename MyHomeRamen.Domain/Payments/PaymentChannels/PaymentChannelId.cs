using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Payments.PaymentChannels;

public readonly record struct PaymentChannelId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentChannelId id) => id.Value;

    public static implicit operator PaymentChannelId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
