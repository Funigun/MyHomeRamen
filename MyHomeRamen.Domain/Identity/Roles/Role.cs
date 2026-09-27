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

    public static Role CreateAdmin(IEnumerable<PermissionId> permissions)
    => CreateSystemRole(RoleConstants.RestaurantAdmin, "Administrator role with full access to the system.", permissions);
    public static Role CreateGuest(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.Guest, "Guest role with limited access to the system.", permissions);

    public static Role CreateCustomer(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.Customer, "Customer role with extended access to the system.", permissions);

    public static Role CreateCompanyOwner(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.CompanyOwner, "Company Owner role with extended access to the system.", permissions);

    public static Role CreateCustom(string name, string description)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description
        };

        return role;
    }

    public static Role CreateCustom(string name, string description, IEnumerable<PermissionId> permissions)
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
    
    public void UpdateDescription(string description) => Description = description;

    public void UpdatePermissions(IEnumerable<PermissionId> permissions)
    {
        _permissions.Clear();

        foreach (PermissionId permission in permissions)
        {
            _permissions.Add(RolePermission.Create(Id, permission));
        }
    }

    private static Role CreateSystemRole(string name, string description, IEnumerable<PermissionId> permissions)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description,
            IsRemovable = false,
            IsEditable = false
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        return role;
    }
}
