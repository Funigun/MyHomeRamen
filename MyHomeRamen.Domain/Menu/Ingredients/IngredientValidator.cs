using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Domain.Menu.Ingredients;

internal static class IngredientValidator
{
    internal static void Validate(Ingredient ingredient)
    {
        CheckName(ingredient);
        CheckDescription(ingredient);
        CheckPrice(ingredient);
        CheckCategories(ingredient);
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

    private static void CheckCategories(Ingredient ingredient)
    {
        if (ingredient.Categories.Any(category => category.CategoryType != CategoryType.Ingredient))
        {
            throw IngredientErrors.CategoryTypeNotValid();
        }

        if (ingredient.Categories.DistinctBy(category => category.Id).Count() != ingredient.Categories.Count)
        {
            throw IngredientErrors.CategoriesNotUnique();
        }
    }
}
