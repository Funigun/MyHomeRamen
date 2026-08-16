using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

public readonly record struct BasketId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BasketId id) => id.Value;

    public static implicit operator BasketId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
