using MyHomeRamen.Domain.Common;

namespace MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

public static class RestaurantIngredientPriceErrors
{
    public static DomainException IdentifierRequired()
        => new("Price, restaurant, and ingredient identifiers must not be empty.");

    public static DomainException IngredientRequired()
        => new("Priced ingredient is required.");

    public static DomainException PriceMustNotBeNegative()
        => new("Extra ingredient price must be greater than or equal to zero.");
}
