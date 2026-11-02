namespace MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

internal static class RestaurantIngredientPriceValidator
{
    internal static void Validate(RestaurantIngredientPrice price)
    {
        if (price.Id.Value == Guid.Empty || price.RestaurantId == Guid.Empty || price.IngredientId.Value == Guid.Empty)
        {
            throw RestaurantIngredientPriceErrors.IdentifierRequired();
        }

        if (price.Ingredient is null)
        {
            throw RestaurantIngredientPriceErrors.IngredientRequired();
        }

        if (price.Price < 0)
        {
            throw RestaurantIngredientPriceErrors.PriceMustNotBeNegative();
        }
    }
}
