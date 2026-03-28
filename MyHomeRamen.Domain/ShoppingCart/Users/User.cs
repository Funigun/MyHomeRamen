using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    private User()
    {
    }

    private User(UserId id, List<Role> roles, List<Permission> permissions)
    {
        Id = id;
        _roles = roles;
        _permissions = permissions;
    }

    public static User Create(UserId id, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, roles, permissions);

        return user;
    }
}
