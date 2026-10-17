using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Domain.Identity.CompanyMemberships;

public sealed class CompanyMembership : AuditableEntity, IEntity<CompanyMembershipId>
{
    public CompanyMembershipId Id { get; private set; }
    public UserId UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid? RestaurantId { get; private set; }

    private CompanyMembership() { }

    public static CompanyMembership Create(UserId userId, Guid companyId, Guid? restaurantId = null)
    {
        CompanyMembership membership = new()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            CompanyId = companyId,
            RestaurantId = restaurantId
        };

        CompanyMembershipValidator.ValidateCompanyMembership(membership);
        return membership;
    }
}
