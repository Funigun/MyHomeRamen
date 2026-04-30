using Microsoft.AspNetCore.Identity;
using MyHomeRamen.Domain.Common.Address;

namespace MyHomeRamen.Domain.Users;

public class User : IdentityUser<Guid>
{
    private readonly List<Address> _addresses = [];

    public Guid RestaurantId { get; private set; }

    public string? KeycloakUserId { get; private set; }

    public Guid? GuestId { get; private set; }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public string Role { get; private set; } = default!;

    public ICollection<Address> Addresses => _addresses.ToList();

    private User()
    {
    }

    public static User Create(string keycloakUserId, string userName, string firstName, string lastName, string email, string phoneNumber, string role)
    {
        User user = new User
        {
            Id = Guid.CreateVersion7(),
            KeycloakUserId = keycloakUserId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            UserName = userName,
            Role = role
        };

        UserValidator.ValidateUser(user);
        return user;
    }

    public static User CreateGuest()
    {
        User user = new User
        {
            Id = Guid.CreateVersion7(),
            GuestId = Guid.CreateVersion7(),
            FirstName = "Guest",
            LastName = "User",
            Role = "Guest"
        };

        UserValidator.ValidateUser(user);
        return user;
    }

    public void SetRestaurantId(Guid restaurantId)
    {
        RestaurantId = restaurantId;
    }

    public void AddAddress(Address address)
    {
        if (_addresses.Count >= AddressConstants.MaxAddressesPerUser)
        {
            throw AddressErrors.MaxAddressesReached();
        }

        if (address.IsDefault)
        {
            Address? currentDefault = _addresses.FirstOrDefault(a => a.IsDefault);
            currentDefault?.UnsetDefault();
        }
        else if (_addresses.Count == 0)
        {
            address.SetAsDefault();
        }

        _addresses.Add(address);
    }

    public void UpdateAddress(Guid addressId, string street, string building, string apartment, string city, string zipCode, bool isDefault)
    {
        Address? address = _addresses.FirstOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            throw AddressErrors.AddressNotFound();
        }

        address.Update(street, building, apartment, city, zipCode);

        if (isDefault && !address.IsDefault)
        {
            Address? currentDefault = _addresses.FirstOrDefault(a => a.IsDefault);
            currentDefault?.UnsetDefault();
            address.SetAsDefault();
        }
        else if (!isDefault && address.IsDefault)
        {
            address.UnsetDefault();
        }
    }

    public void RemoveAddress(Guid addressId)
    {
        Address? address = _addresses.FirstOrDefault(a => a.Id == addressId);

        if (address is null)
        {
            throw AddressErrors.AddressNotFound();
        }

        _addresses.Remove(address);
    }
}
