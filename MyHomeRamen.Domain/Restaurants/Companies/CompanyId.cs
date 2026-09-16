using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public record struct CompanyId(Guid Value) : IEntityId
{
    public static implicit operator Guid(CompanyId id) => id.Value;

    public static implicit operator CompanyId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
