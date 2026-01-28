namespace MyHomeRamen.Domain.Orders;

public sealed class UserAddress
{
    public string Street { get; private set; }

    public string Building { get; private set; }

    public string Apartment { get; private set; }

    public string City { get; private set; }

    public string ZipCode { get; private set; }

    private UserAddress()
    {
    }

    public static UserAddress Create(string street, string city, string building, string apartment, string zipCode)
    {
        return new UserAddress
        {
            Street = street,
            City = city,
            Building = building,
            Apartment = apartment,
            ZipCode = zipCode
        };
    }
}
