using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Users;

public sealed class Role : AuditableEntity, IEntity<RoleId>
{
    private readonly List<Permission> _permissions = [];

    public RoleId Id { get; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public ICollection<Permission> Permissions => _permissions.ToList();

    private Role()
    {
    }

    private Role(RoleId id, Guid restaurantId, List<Permission> permissions)
    {
        Id = id;
        RestaurantId = restaurantId;
        _permissions = permissions;
    }

    public static Role CreateForSeed(RoleId id, Guid restaurantId, string name)
    {
        Role role = new(id, restaurantId, [])
        {
            Name = name,
            Description = name
        };

        return role;
    }

    public static Role CreateCustomerRole(RoleId id, Guid restaurantId, List<Permission> permissions)
    {
        return new(id, restaurantId, permissions)
        {
            Name = RoleConstants.Customer,
            Description = "A customer"
        };
    }
}
