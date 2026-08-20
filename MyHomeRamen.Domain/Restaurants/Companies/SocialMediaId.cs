using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public record struct SocialMediaId(Guid Value) : IEntityId
{
    public static implicit operator Guid(SocialMediaId id) => id.Value;

    public static implicit operator SocialMediaId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
