using MyHomeRamen.Domain.Common.Address;

namespace MyHomeRamen.Domain.Users;

public class Address
{
    public Guid Id { get; private set; }

    public string Street { get; private set; } = default!;

    public string Building { get; private set; } = default!;

    public string Apartment { get; private set; } = default!;

    public string City { get; private set; } = default!;

    public string ZipCode { get; private set; } = default!;

    public bool IsDefault { get; private set; }

    private Address()
    {
    }

    public static Address Create(Guid id, string street, string building, string apartment, string city, string zipCode, bool isDefault)
    {
        Address address = new()
        {
            Id = id,
            Street = street,
            Building = building,
            Apartment = apartment,
            City = city,
            ZipCode = zipCode,
            IsDefault = isDefault
        };

        AddressValidator.ValidateAddress(address);

        return address;
    }

    public void Update(string street, string building, string apartment, string city, string zipCode)
    {
        Street = street;
        Building = building;
        Apartment = apartment;
        City = city;
        ZipCode = zipCode;

        AddressValidator.ValidateAddress(this);
    }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void UnsetDefault()
    {
        IsDefault = false;
    }
}
