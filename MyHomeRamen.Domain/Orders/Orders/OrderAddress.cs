namespace MyHomeRamen.Domain.Orders.Orders;

public sealed class OrderAddress
{
    public string Street { get; private set; } = default!;

    public string Building { get; private set; } = default!;

    public string Apartment { get; private set; } = default!;

    public string City { get; private set; } = default!;

    public string ZipCode { get; private set; } = default!;

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
