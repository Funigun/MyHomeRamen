namespace MyHomeRamen.Domain.Common.Order;

public static class OrderErrors
{
    public static DomainException CompanyRequired()
        => new("Payment order company identifier must not be empty.");

    public static DomainException RestaurantRequired()
        => new("Order must have a restaurant assigned.");

    public static DomainException RestaurantUnavailable()
        => new("Order restaurant is unavailable.");

    public static DomainException UserRequired()
        => new("Order must have a user assigned.");

    public static DomainException CurrencyRequired()
        => new("Order currency must be a three-letter code.");

    public static DomainException InvalidOrderLine()
        => new("Order line must include an offer, a positive quantity, and a non-negative charged price.");

    public static DomainException OrderMustHaveProducts()
        => new("Order must have at least one product.");

    public static DomainException TooManyProducts()
        => new($"Order cannot have more than {OrderConstants.MaxProductsCount} products.");

    public static DomainException AmountTooSmall()
        => new($"Order amount cannot be smaller than {OrderConstants.MinTotalAmount}.");

    public static DomainException AmountTooLarge()
        => new($"Order amount cannot be larger than {OrderConstants.MaxTotalAmount}.");

    public static DomainException DeliveryAmountTooSmall()
        => new($"Order total amount for delivery cannot be smaller than {OrderConstants.MinDeliveryAmount}.");

    public static DomainException DeliveryAddressRequired()
        => new("Delivery order must include a delivery address.");
}
