using Microsoft.AspNetCore.Identity;

namespace MyHomeRamen.Identity.Api.Domain;

public class User : IdentityUser<Guid>
{
    public Guid RestgaurantId { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public Address? Address { get; private set; }

    private User()
    {
    }

    public static User Create(Guid restaurantId, string userName, string firstName, string lastName, string email, string phoneNumber)
    {
        return new User
        {
            RestgaurantId = restaurantId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserName = userName
        };
    }
}
