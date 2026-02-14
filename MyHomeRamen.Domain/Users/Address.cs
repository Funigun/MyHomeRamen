namespace MyHomeRamen.Domain.Users;

public class Address
{
    public Guid Id { get; private set; }

    public string Street { get; private set; }

    public string Building { get; private set; }

    public string Apartment { get; private set; }

    public string City { get; private set; }

    public string ZipCode { get; private set; }

    private Address()
    {
    }

    public static Address Create(Guid id, string street, string city, string building, string apartment, string zipCode)
    {
        return new Address
        {
            Id = id,
            Street = street,
            City = city,
            Building = building,
            Apartment = apartment,
            ZipCode = zipCode
        };
    }
}
