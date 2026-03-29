namespace MyHomeRamen.Domain.Common.Order;

public static class OrderErrors
{
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
}
