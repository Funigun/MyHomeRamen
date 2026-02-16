using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders.Users;

public sealed class Permission : AuditableEntity, IEntity<PermissionId>
{
    public PermissionId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    private Permission()
    {
    }

    private Permission(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Permission Create(string name, string description)
    {
        return new Permission(name, description);
    }
}
