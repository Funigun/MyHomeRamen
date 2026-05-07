namespace MyHomeRamen.Domain.Common.Basket;

public static class BasketErrors
{
    public static DomainException BasketUserRequired()
        => new("Basket must have a user assigned.");

    public static DomainException BasketItemProductRequired()
        => new("Basket item must have a product assigned.");

    public static DomainException BasketItemQuantityInvalid()
        => new("Basket item quantity must be at least 1.");

    public static DomainException BasketItemPriceInvalid()
        => new("Basket item price must be a non-negative value.");

    public static DomainException BasketItemsLimitReached()
        => new($"Basket cannot contain more than {BasketConstants.MaxProductsCount} items.");
}
