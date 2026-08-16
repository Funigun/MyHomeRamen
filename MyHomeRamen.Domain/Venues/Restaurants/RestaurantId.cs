using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Venues.Restaurants;

public record struct RestaurantId(Guid Value) : IEntityId
{
    public static implicit operator Guid(RestaurantId id) => id.Value;

    public static implicit operator RestaurantId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
