using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Domain.Identity.CompanyMemberships;

public sealed class CompanyMembership : AuditableEntity, IEntity<CompanyMembershipId>
{
    public CompanyMembershipId Id { get; private set; }
    public UserId UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid RestaurantId { get; private set; }
    public RoleId RoleId { get; private set; }
    public bool IsActive { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;

    private CompanyMembership() { }

    public static CompanyMembership CreateRestaurantAdmin(UserId userId, Guid companyId, Guid restaurantId, RoleId roleId, string idempotencyKey)
    {
        if (companyId == Guid.Empty || restaurantId == Guid.Empty || roleId.Value == Guid.Empty || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new DomainException("Company, restaurant, role, and idempotency key are required.");
        }

        return new CompanyMembership
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            CompanyId = companyId,
            RestaurantId = restaurantId,
            RoleId = roleId,
            IsActive = true,
            IdempotencyKey = idempotencyKey
        };
    }

    public static CompanyMembership CreateOwner(UserId userId, Guid companyId, RoleId roleId, string idempotencyKey)
    {
        if (companyId == Guid.Empty || roleId.Value == Guid.Empty || string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new DomainException("Company, role, and idempotency key are required.");
        }

        return new CompanyMembership
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId,
            IsActive = true,
            IdempotencyKey = idempotencyKey
        };
    }
}
