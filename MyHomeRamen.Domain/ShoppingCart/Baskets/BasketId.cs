using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

public readonly record struct BasketId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BasketId id) => id.Value;

    public static implicit operator BasketId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
