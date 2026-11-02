using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

public readonly record struct RestaurantIngredientPriceId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RestaurantIngredientPriceId id) => id.Value;

    public static implicit operator RestaurantIngredientPriceId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
