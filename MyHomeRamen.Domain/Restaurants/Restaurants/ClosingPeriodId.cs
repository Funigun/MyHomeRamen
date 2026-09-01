using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Restaurants.Restaurants;

public record struct ClosingPeriodId(Guid Value) : IEntityId
{
    public static implicit operator Guid(ClosingPeriodId id) => id.Value;

    public static implicit operator ClosingPeriodId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
