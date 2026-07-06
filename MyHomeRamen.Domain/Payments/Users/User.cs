using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Domain.Payments.Roles;

namespace MyHomeRamen.Domain.Payments.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

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

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, roles, permissions)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };

        UserValidator.Validate(user);

        return user;
    }
}
