using System.Collections.ObjectModel;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

internal static class Mappings
{
    public static Product ToDomain(this CreateProductRequest request, Category category, IEnumerable<Ingredient> ingredients, IEnumerable<Ingredient> customIngredients)
    {
        return Product.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description ?? string.Empty,
            request.Price,
            string.Empty,
            new Collection<Ingredient>(ingredients.ToList()),
            new Collection<Ingredient>(customIngredients.ToList()),
            [category]);
    }
}
