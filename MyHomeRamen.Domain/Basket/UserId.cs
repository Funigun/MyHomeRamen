using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public readonly record struct UserId(Guid Value) : IEntityId
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
