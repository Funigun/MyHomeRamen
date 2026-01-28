using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public UserAddress Address { get; private set; }

    private User()
    {
    }

    private User(UserId id, UserAddress address)
    {
        Id = id;
        Address = address;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, UserAddress address)
    {
        return new User(id, address)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
}
