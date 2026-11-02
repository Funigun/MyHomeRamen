namespace MyHomeRamen.Domain.Common.Basket;

public static class BasketErrors
{
    public static DomainException BasketUserRequired()
        => new("Basket must have a user assigned.");

    public static DomainException BasketRestaurantRequired()
        => new("Basket must have a restaurant assigned.");

    public static DomainException BasketItemProductRequired()
        => new("Basket item must have a product assigned.");

    public static DomainException BasketOfferRequired()
        => new("Basket item must have a restaurant offer assigned.");

    public static DomainException RestaurantUnavailable()
        => new("Restaurant is unavailable.");

    public static DomainException BasketRestaurantMismatch()
        => new("Requested restaurant does not match the active basket.");

    public static DomainException BasketNotReadyForCheckout()
        => new("Basket must contain items and shipping details before checkout.");

    public static DomainException OfferUnavailable()
        => new("Restaurant offer is unavailable or no longer active.");

    public static DomainException BasketItemQuantityInvalid()
        => new("Basket item quantity must be at least 1.");

    public static DomainException BasketItemsLimitReached()
        => new($"Basket cannot contain more than {BasketConstants.MaxProductsCount} items.");

    public static DomainException ItemNotFound()
        => new("Basket item was not found.");

    public static DomainException BasketNotActive()
        => new("Basket is not active.");
}
