using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public readonly record struct ShoppingCartId(Guid Value) : IEntityId
{
    public static implicit operator Guid(ShoppingCartId id) => id.Value;
    public static implicit operator ShoppingCartId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
