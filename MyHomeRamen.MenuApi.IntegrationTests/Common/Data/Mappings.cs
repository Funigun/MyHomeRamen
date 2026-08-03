using MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;
using MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;
using MyHomeRamen.Features.Menu.Features.Products.CreateProduct;
using MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

internal static class Mappings
{
    internal static CreateCategoryRequest ToCreateCategoryRequest(this Category category) =>
        new(
            category.Name,
            (int)category.CategoryType
        );

    internal static CreateProductRequest ToCreateProductRequest(this Product product) =>
        new(
            product.Name,
            product.Description,
            product.Price,
            product.Categories[0].Id,
            product.BaseIngredients.Select(i => (Guid)i.Id),
            product.CustomIngredients.Select(i => (Guid)i.Id)
        );

    internal static UpdateIngredientRequest ToUpdateIngredientRequest(this Ingredient ingredient) =>
        new(
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => (Guid)c.Id)
        );

    internal static UpdateProductRequest ToUpdateProductRequest(this Product product) =>
        new(
            product.Name,
            product.Description,
            product.Price,
            product.Categories[0].Id,
            product.BaseIngredients.Select(i => (Guid)i.Id),
            product.CustomIngredients.Select(i => (Guid)i.Id)
        );
}
