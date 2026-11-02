using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Menu.Offers;

public static class RestaurantProductOfferErrors
{
    public static DomainException IdentifierRequired()
        => new("Offer, restaurant, and company identifiers must not be empty.");

    public static DomainException ProductRequired()
        => new("Offer product is required.");

    public static DomainException CompanyMismatch()
        => new("Offer and product must belong to the same company.");

    public static DomainException PriceMustBePositive()
        => new("Offer price must be greater than zero.");
}
