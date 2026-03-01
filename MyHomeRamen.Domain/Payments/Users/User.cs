using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Payments.Payments;

namespace MyHomeRamen.Domain.Payments.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];
    private readonly List<Payment> _payments = [];

    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public Payment DefaultMethod { get; private set; }

    public ICollection<Payment> Payments => _payments.ToList();

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    private User()
    {
    }

    private User(UserId id, Payment defaultMethod, List<Role> roles, List<Permission> permissions)
    {
        Id = id;
        DefaultMethod = defaultMethod;
        _roles = roles;
        _permissions = permissions;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, Payment defaultMethod, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, defaultMethod, roles, permissions)
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
