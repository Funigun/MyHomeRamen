using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Payments;

public sealed class User : AuditableEntity, IEntity<UserId>
{
    public UserId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public Payment DefaultMethod { get; private set; }

    private User()
    {
    }

    private User(UserId id, Payment defaultMethod)
    {
        Id = id;
        DefaultMethod = defaultMethod;
    }

    public static User Create(UserId id, string firstName, string lastName, string email, string phoneNumber, Payment defaultMethod)
    {
        return new User(id, defaultMethod)
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber
        };
    }
}
