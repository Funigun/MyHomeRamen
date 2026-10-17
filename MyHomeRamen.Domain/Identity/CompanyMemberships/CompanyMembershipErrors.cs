using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Identity.CompanyMemberships;

public static class CompanyMembershipErrors
{
    public static DomainException UserRequired() => new("Company membership user is required.");
    public static DomainException CompanyRequired() => new("Company membership company is required.");
    public static DomainException RestaurantIdCannotBeEmpty() => new("Company membership restaurant identifier cannot be empty.");
}
