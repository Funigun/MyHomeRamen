namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

public sealed class Address
{
    public string Street { get; private set; } = default!;

    public string City { get; private set; } = default!;

    public string ZipCode { get; private set; } = default!;

    public Location Location { get; private set; } = default!;

    private Address()
    {
    }

    public static Address Create(string street, string city, string zipCode, Location location)
    {
        Address address = new()
        {
            Street = street,
            City = city,
            ZipCode = zipCode,
            Location = location
        };

        AddressValidator.Validate(address);
        return address;
    }
}
