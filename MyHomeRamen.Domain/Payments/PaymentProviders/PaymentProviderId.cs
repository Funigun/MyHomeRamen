using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.PaymentProviders;

public readonly record struct PaymentProviderId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentProviderId id) => id.Value;

    public static implicit operator PaymentProviderId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
