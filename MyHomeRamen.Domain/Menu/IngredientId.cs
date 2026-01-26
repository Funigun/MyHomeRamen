using System;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public readonly record struct IngredientId(Guid Value) : IEntityId
{
    public static implicit operator Guid(IngredientId id) => id.Value;
    public static implicit operator IngredientId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
