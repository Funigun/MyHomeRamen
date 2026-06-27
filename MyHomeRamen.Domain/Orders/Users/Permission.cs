using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Orders.Users;

public sealed class Permission : AuditableEntity, IEntity<PermissionId>
{
    public PermissionId Id { get; private set; }

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

    private Permission(PermissionId id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public static Permission CreateForSeed(PermissionId id, string name)
    {
        return new Permission(id, name, name);
    }

    public static Permission Create(string name, string description)
    {
        return new Permission(name, description);
    }
}
