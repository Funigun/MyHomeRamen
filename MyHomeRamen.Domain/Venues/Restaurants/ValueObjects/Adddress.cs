namespace MyHomeRamen.Domain.Venues.Restaurants.ValueObjects;

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
        return new Address
        {
            Street = street,
            City = city,
            ZipCode = zipCode,
            Location = location
        };
    }
}
