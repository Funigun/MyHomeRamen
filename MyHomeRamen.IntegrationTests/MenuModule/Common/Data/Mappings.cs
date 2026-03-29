using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

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
            product.BaseIngredients.Select(i => (Guid)i.Id)
        );

    internal static CreateIngredientRequest ToCreateIngredientRequest(this Ingredient ingredient) =>
        new(
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => c.Id.Value).ToList()
        );
}
