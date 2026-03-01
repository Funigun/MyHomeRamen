using Microsoft.AspNetCore.Identity;

namespace MyHomeRamen.Domain.Users;

public class User : IdentityUser<Guid>
{
    private readonly List<Address> _addresses = [];

    public Guid RestaurantId { get; private set; }

    public string KeycloakUserId { get; private set; } = default!;

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Role { get; private set; }

    public ICollection<Address> Addresses => _addresses.ToList();

    private User()
    {
    }

    public static User Create(Guid restaurantId, string keycloakUserId, string userName, string firstName, string lastName, string email, string phoneNumber, string role)
    {
        return new User
        {
            Id = Guid.CreateVersion7(),
            KeycloakUserId = keycloakUserId,
            RestaurantId = restaurantId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserName = userName,
            Role = role
        };
    }
}
