using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Reservations.Bookings;

namespace MyHomeRamen.Domain.Reservations.Users;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Booking> _bookings = [];
    private readonly List<Role> _roles = [];
    private readonly List<Permission> _permissions = [];

    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public IReadOnlyList<Booking> Bookings => _bookings.ToList();

    public ICollection<Role> Roles => _roles.ToList();

    public ICollection<Permission> Permissions => _permissions.ToList();

    private User()
    {
    }

    private User(UserId id, List<Booking> bookings, List<Role> roles, List<Permission> permissions)
    {
        Id = id;
        _bookings = bookings;
        _roles = roles;
        _permissions = permissions;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, List<Booking> bookings, List<Role> roles, List<Permission> permissions)
    {
        User user = new(id, bookings, roles, permissions)
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
