using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Domain.Identity.Roles;

public class Role : AuditableEntity, IEntity<RoleId>
{
    private readonly List<RolePermission> _permissions = [];

    public RoleId Id { get; private set; }

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public bool IsRemovable { get; private set; } = true;

    public bool IsEditable { get; private set; } = true;

    public IReadOnlyCollection<RolePermission> RolePermissions => _permissions.ToList();

    private Role()
    {
        
    }

    public static Role Create(string name, string description)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description
        };

        return role;
    }

    public static Role Create(string name, string description, IEnumerable<PermissionId> permissions)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        return role;
    }

    public static Role CreateAdmin(IEnumerable<PermissionId> permissions)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = RoleConstants.Admin,
            Description = "Administrator role with full access to the system.",
            IsRemovable = false,
            IsEditable = false
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        return role;
    }

    public static Role CreateGuest(IEnumerable<PermissionId> permissions)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = RoleConstants.Guest,
            Description = "Guest role with limited access to the system.",
            IsRemovable = false,
            IsEditable = false
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        return role;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdatePermissions(IEnumerable<PermissionId> permissions)
    {
        _permissions.Clear();

        foreach (PermissionId permission in permissions)
        {
            _permissions.Add(RolePermission.Create(Id, permission));
        }
    }
}
