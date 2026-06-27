using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.Users;

public sealed class Role : AuditableEntity, IEntity<RoleId>
{
    private readonly List<Permission> _permissions = [];

    public RoleId Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public ICollection<Permission> Permissions => _permissions.ToList();

    private Role()
    {
    }

    private Role(RoleId id, List<Permission> permissions)
    {
        Id = id;
        _permissions = permissions;
    }

    public static Role CreateForSeed(RoleId id, string name, List<Permission> permissions)
    {
        return new(id, permissions)
        {
            Name = name,
            Description = name
        };
    }

    public static Role CreateEmployeeRole(RoleId roleId, List<Permission> validPermissions)
    {
        return new Role(roleId, validPermissions)
        {
            Name = RoleConstants.Employee,
            Description = "Employee role with permissions to manage orders and view reports."
        };
    }
}
