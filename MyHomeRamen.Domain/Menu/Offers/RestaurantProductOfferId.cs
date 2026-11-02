using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.Offers;

public readonly record struct RestaurantProductOfferId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RestaurantProductOfferId id) => id.Value;

    public static implicit operator RestaurantProductOfferId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
