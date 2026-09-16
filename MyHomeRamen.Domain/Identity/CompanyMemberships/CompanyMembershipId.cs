using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.CompanyMemberships;

public readonly record struct CompanyMembershipId(Guid Value) : IEntityId
{
    public static implicit operator Guid(CompanyMembershipId id) => id.Value;
    public static implicit operator CompanyMembershipId(Guid value) => new(value);
}
