using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Identity.Roles;

namespace MyHomeRamen.Domain.Identity.Permissions;

public sealed class Permission : Aggregate<PermissionId>
{
    private readonly List<RolePermission> _roles = [];

    public string Name { get; private set; } = default!;

    public string Description { get; private set; } = default!;

    public string Module { get; private set; } = default!;

    public bool IsResourceScoped { get; private set; }

    public IReadOnlyCollection<RolePermission> RolePermissions => _roles.ToList();

    private Permission() { }

    public static Permission Create(string name, string description, string module, bool isResourceScoped = false)
    {
        Permission? permission = new()
        {
            Id = new PermissionId(Guid.NewGuid()),
            Name = name,
            Description = description,
            Module = module,
            IsResourceScoped = isResourceScoped,
        };

        return permission;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateIsResourceScoped(bool isResourceScoped)
    {
        IsResourceScoped = isResourceScoped;
    }
}
