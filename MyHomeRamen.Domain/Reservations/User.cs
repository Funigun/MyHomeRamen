using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Reservations;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    private readonly List<Booking> _bookings = [];

    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public IReadOnlyList<Booking> Bookings => _bookings.ToList();

    private User()
    {
    }

    private User(UserId id, List<Booking> bookings)
    {
        Id = id;
        _bookings = bookings;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, List<Booking> bookings)
    {
        return new User(id, bookings)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
}
