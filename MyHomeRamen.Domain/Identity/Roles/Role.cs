using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Identity.Permissions;

namespace MyHomeRamen.Domain.Identity.Roles;

public class Role : AuditableEntity, IEntity<RoleId>
{
    private readonly List<RolePermission> _permissions = [];
    private readonly List<UserRole> _userRoles = [];

    public RoleId Id { get; private set; }

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public RoleScope Scope { get; private set; }

    public Guid? RestaurantId { get; private set; }

    public bool IsRemovable { get; private set; } = true;

    public bool IsEditable { get; private set; } = true;

    public IReadOnlyCollection<RolePermission> RolePermissions => _permissions.ToList();

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.ToList();

    private Role() { }

    private Role(RoleId id, RoleScope scope, Guid? restaurantId, List<RolePermission> permissions, List<UserRole> userRoles)
    {
        Id = id;
        Scope = scope;
        RestaurantId = restaurantId;
        _permissions = permissions;
        _userRoles = userRoles;
    }

    public static Role CreateAdmin(IEnumerable<PermissionId> permissions, Guid restaurantId)
        => CreateSystemRole(RoleConstants.RestaurantAdmin, "Administrator role with access to one restaurant.", permissions, RoleScope.Restaurant, restaurantId);

    public static Role CreateGuest(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.Guest, "Guest role with limited access to the system.", permissions, RoleScope.Company, null);

    public static Role CreateCustomer(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.Customer, "Customer role with extended access to the system.", permissions, RoleScope.Company, null);

    public static Role CreateCompanyOwner(IEnumerable<PermissionId> permissions)
        => CreateSystemRole(RoleConstants.CompanyOwner, "Company Owner role with extended access to the system.", permissions, RoleScope.Company, null);

    public static Role CreateCustom(string name, string description, RoleScope scope, Guid? restaurantId = null)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description,
            Scope = scope,
            RestaurantId = restaurantId
        };

        RoleValidator.ValidateRole(role);
        return role;
    }

    public static Role CreateCustom(
        string name,
        string description,
        IEnumerable<PermissionId> permissions,
        RoleScope scope,
        Guid? restaurantId = null)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description,
            Scope = scope,
            RestaurantId = restaurantId
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        RoleValidator.ValidateRole(role);
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

    private static Role CreateSystemRole(
        string name,
        string description,
        IEnumerable<PermissionId> permissions,
        RoleScope scope,
        Guid? restaurantId)
    {
        Role role = new()
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Description = description,
            Scope = scope,
            RestaurantId = restaurantId,
            IsRemovable = false,
            IsEditable = false
        };

        foreach (PermissionId permission in permissions)
        {
            role._permissions.Add(RolePermission.Create(role.Id, permission));
        }

        RoleValidator.ValidateRole(role);
        return role;
    }
}
