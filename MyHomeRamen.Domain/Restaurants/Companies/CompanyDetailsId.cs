using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Restaurants.Companies;

public record struct CompanyDetailsId(Guid Value) : IEntityId
{
    public static implicit operator Guid(CompanyDetailsId id) => id.Value;

    public static implicit operator CompanyDetailsId(Guid value) => new(value);

    public override readonly string ToString() => Value.ToString();
}
