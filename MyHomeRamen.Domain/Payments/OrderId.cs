using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments;

public readonly record struct OrderId(Guid Value) : IEntityId
{
    public static implicit operator Guid(OrderId id) => id.Value;
    public static implicit operator OrderId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
