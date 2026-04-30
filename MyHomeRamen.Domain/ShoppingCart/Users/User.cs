using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    public bool IsGuest { get; private set; }

    private User()
    {
    }

    private User(UserId id, List<Role> roles, List<Permission> permissions, bool isGuest)
    {
        Id = id;
        _roles = roles;
        _permissions = permissions;
        IsGuest = isGuest;
    }

    public static User Create(UserId id, List<Role> roles, List<Permission> permissions, bool isGuest = false)
    {
        User user = new(id, roles, permissions, isGuest);

        return user;
    }

    public static User CreateGuest(UserId id)
    {
        User user = new(id, new List<Role>(), new List<Permission>(), isGuest: true);
        return user;
    }
}
