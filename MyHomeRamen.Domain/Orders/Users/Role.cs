using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders.Users;

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
        Role role = new(id, permissions)
        {
            Name = name,
            Description = name
        };

        return role;
    }

    public static Role CreateCustomerRole(RoleId id, List<Permission> permissions)
    {
        return new(id, permissions)
        {
            Name = RoleConstants.Customer,
            Description = "A customer"
        };
    }
}
