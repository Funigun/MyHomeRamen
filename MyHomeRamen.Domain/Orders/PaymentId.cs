using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public readonly record struct PaymentId(Guid Value) : IEntityId
{
    public static implicit operator Guid(PaymentId id) => id.Value;
    
    public static implicit operator PaymentId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
