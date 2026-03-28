using MyHomeRamen.Domain.Common.Ingredient;

namespace MyHomeRamen.Domain.Orders.Ingredients;

internal static class IngredientValidator
{
    internal static void Validate(Ingredient ingredient)
    {
        CheckName(ingredient);
        CheckPrice(ingredient);
    }

    private static void CheckName(Ingredient ingredient)
    {
        if (ingredient.Name.Length < IngredientConstants.MinNameLength)
        {
            throw IngredientErrors.NameTooShort();
        }

        if (ingredient.Name.Length > IngredientConstants.MaxNameLength)
        {
            throw IngredientErrors.NameTooLong();
        }
    }

    private static void CheckPrice(Ingredient ingredient)
    {
        if (ingredient.OriginalPrice < IngredientConstants.MinPrice)
        {
            throw IngredientErrors.PriceTooSmall();
        }

        if (ingredient.OriginalPrice > IngredientConstants.MaxPrice)
        {
            throw IngredientErrors.PriceTooHigh();
        }
    }
}
