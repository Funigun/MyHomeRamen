namespace MyHomeRamen.Domain.ShoppingCart.ShippingDetails;

public sealed class ShippingDetails
{
    public bool PersonalPickup { get; private set; }

    public bool Delivery { get; private set; }

    public ShippingAddress? ShippingAddress { get; private set; }

    private ShippingDetails() { }

    public static ShippingDetails CreateDelivery(ShippingAddress address)
    {
        return new ShippingDetails
        {
            PersonalPickup = false,
            Delivery = true,
            ShippingAddress = address
        };
    }

    public static ShippingDetails CreatePersonalPickup()
    {
        return new ShippingDetails
        {
            PersonalPickup = true,
            Delivery = false,
            ShippingAddress = null
        };
    }
}
