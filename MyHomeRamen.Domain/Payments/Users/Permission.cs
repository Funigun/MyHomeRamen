using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments.Users;

public sealed class Permission : AuditableEntity, IEntity<PermissionId>
{
    public PermissionId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    private Permission()
    {
    }

    private Permission(PermissionId id, Guid restaurantId, string name, string description)
    {
        Id = id;
        RestaurantId = restaurantId;
        Name = name;
        Description = description;
    }

    public static Permission Create(PermissionId id, Guid restaurantId, string name, string description)
    {
        return new Permission(id, restaurantId, name, description);
    }
}
