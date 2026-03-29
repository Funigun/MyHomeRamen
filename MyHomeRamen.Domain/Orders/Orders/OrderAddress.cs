namespace MyHomeRamen.Domain.Orders.Orders;

public sealed class OrderAddress
{
    public string Street { get; private set; }

    public string Building { get; private set; }

    public string Apartment { get; private set; }

    public string City { get; private set; }

    public string ZipCode { get; private set; }

    private OrderAddress()
    {
    }

    public static OrderAddress Create(string street, string city, string building, string apartment, string zipCode)
    {
        return new OrderAddress
        {
            Street = street,
            City = city,
            Building = building,
            Apartment = apartment,
            ZipCode = zipCode
        };
    }
}
