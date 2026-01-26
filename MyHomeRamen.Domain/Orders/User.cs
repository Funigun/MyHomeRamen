using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    private User()
    {
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber)
    {
        return new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
}
