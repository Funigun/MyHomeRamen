using MyHomeRamen.Domain.Common.Ingredient;

namespace MyHomeRamen.Domain.ShoppingCart.Ingredients;

internal static class IngredientValidator
{
    internal static void Validate(Ingredient ingredient)
    {
        CheckName(ingredient);
        CheckDescription(ingredient);
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

    private static void CheckDescription(Ingredient ingredient)
    {
        if (ingredient.Description.Length < IngredientConstants.MinDescriptionLength)
        {
            throw IngredientErrors.DescriptionTooShort();
        }

        if (ingredient.Description.Length > IngredientConstants.MaxDescriptionLength)
        {
            throw IngredientErrors.DescriptionTooLong();
        }
    }

    private static void CheckPrice(Ingredient ingredient)
    {
        if (ingredient.Price < IngredientConstants.MinPrice)
        {
            throw IngredientErrors.PriceTooSmall();
        }

        if (ingredient.Price > IngredientConstants.MaxPrice)
        {
            throw IngredientErrors.PriceTooHigh();
        }
    }
}
