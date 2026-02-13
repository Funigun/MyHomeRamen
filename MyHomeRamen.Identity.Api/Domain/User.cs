using Microsoft.AspNetCore.Identity;

namespace MyHomeRamen.Identity.Api.Domain;

public class User : IdentityUser<Guid>
{
    private readonly List<Address> _addresses = [];

    public Guid RestaurantId { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public ICollection<Address> Addresses => _addresses.ToList();

    private User()
    {
    }

    public static User Create(Guid restaurantId, string userName, string firstName, string lastName, string email, string phoneNumber)
    {
        return new User
        {
            RestaurantId = restaurantId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserName = userName
        };
    }
}
