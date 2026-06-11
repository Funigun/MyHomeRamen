using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.PaymentMethods;

public readonly record struct PaymentMethodId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentMethodId id) => id.Value;

    public static implicit operator PaymentMethodId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
