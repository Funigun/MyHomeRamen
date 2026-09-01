using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Orders.Ingredients;

public readonly record struct IngredientId(Guid Value) : IEntityId
{
    public static implicit operator Guid(IngredientId id) => id.Value;

    public static implicit operator IngredientId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
