using MyHomeRamen.Domain.Common.Product;

namespace MyHomeRamen.Domain.Orders.Products;

internal static class ProductValidator
{
    internal static void Validate(Product product)
    {
        CheckName(product);
        CheckDescription(product);
        CheckPrice(product);
        CheckIngredients(product);
    }

    private static void CheckName(Product product)
    {
        if (product.Name.Length < ProductConstants.MinNameLength)
        {
            throw ProductErrors.NameTooShort();
        }

        if (product.Name.Length > ProductConstants.MaxNameLength)
        {
            throw ProductErrors.NameTooLong();
        }
    }

    private static void CheckDescription(Product product)
    {
        if (product.Description.Length < ProductConstants.MinDescriptionLength)
        {
            throw ProductErrors.DescriptionTooShort();
        }

        if (product.Description.Length > ProductConstants.MaxDescriptionLength)
        {
            throw ProductErrors.DescriptionTooLong();
        }
    }

    private static void CheckPrice(Product product)
    {
        if (product.Price < ProductConstants.MinPrice)
        {
            throw ProductErrors.PriceTooSmall();
        }

        if (product.Price > ProductConstants.MaxPrice)
        {
            throw ProductErrors.PriceTooHigh();
        }
    }

    private static void CheckIngredients(Product product)
    {
        if (product.BaseIngredients.DistinctBy(ingredient => ingredient.Id).Count() != product.BaseIngredients.Count)
        {
            throw ProductErrors.IngredientsNotUnique();
        }

        if (product.CustomIngredients.DistinctBy(ingredient => ingredient.Id).Count() != product.CustomIngredients.Count)
        {
            throw ProductErrors.CustomIngredientsNotUnique();
        }
    }
}
