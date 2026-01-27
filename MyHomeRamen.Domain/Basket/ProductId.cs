using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public readonly record struct ProductId(Guid Value) : IEntityId
{
    public static implicit operator Guid(ProductId id) => id.Value;
    public static implicit operator ProductId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
