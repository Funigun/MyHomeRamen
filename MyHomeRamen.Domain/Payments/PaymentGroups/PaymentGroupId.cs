using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.PaymentGroups;

public readonly record struct PaymentGroupId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentGroupId id) => id.Value;
    public static implicit operator PaymentGroupId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
