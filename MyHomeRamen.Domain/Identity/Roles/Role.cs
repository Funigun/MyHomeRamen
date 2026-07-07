using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Identity.Roles;

public class Role : AuditableEntity, IEntity<RoleId>
{
    public RoleId Id { get; private set; }

    public Guid RestaurantId { get; private set; }
}
