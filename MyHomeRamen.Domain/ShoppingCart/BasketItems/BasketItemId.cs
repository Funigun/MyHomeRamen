using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.BasketItems;

public readonly record struct BasketItemId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BasketItemId id) => id.Value;

    public static implicit operator BasketItemId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
