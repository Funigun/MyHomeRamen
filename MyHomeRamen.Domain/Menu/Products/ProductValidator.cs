using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Domain.Menu.Products;

internal static class ProductValidator
{
    internal static void ValidateProduct(Product product)
    {
        ProductRules.CheckName(product.Name);
        ProductRules.CheckDescriptionLength(product.Description);
        ProductRules.CheckPriceRange(product.Price);

        if (product.BaseIngredients.DistinctBy(ingredient => ingredient.Id).Count() != product.BaseIngredients.Count)
        {
            throw ProductErrors.IngredientsNotUnique();
        }

        if (product.CustomIngredients.DistinctBy(ingredient => ingredient.Id).Count() != product.CustomIngredients.Count)
        {
            throw ProductErrors.CustomIngredientsNotUnique();
        }

        if (product.Categories.Any(category => category.CategoryType == CategoryType.Ingredient))
        {
            throw ProductErrors.CategoryTypeNotValid();
        }

        if (product.Categories.DistinctBy(category => category.Id).Count() != product.Categories.Count)
        {
            throw ProductErrors.CategoriesNotUnique();
        }
    }
}
