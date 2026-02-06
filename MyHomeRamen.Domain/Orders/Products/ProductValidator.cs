using MyHomeRamen.Domain.Common.Product;

namespace MyHomeRamen.Domain.Orders.Products;

internal static class ProductValidator
{
    internal static void Validate(Product product)
    {
        CheckName(product);
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

    private static void CheckPrice(Product product)
    {
        if (product.OriginalPrice < ProductConstants.MinPrice)
        {
            throw ProductErrors.PriceTooSmall();
        }

        if (product.OriginalPrice > ProductConstants.MaxPrice)
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
