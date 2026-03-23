using System.Collections.ObjectModel;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models.DTOs;

internal static class Mappings
{
    public static Product ToDomain(this CreateProductRequest request, Category category, IEnumerable<Ingredient> ingredients)
    {
        return Product.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description ?? string.Empty,
            request.Price,
            string.Empty,
            new Collection<Ingredient>(ingredients.ToList()),
            [],
             [category]);
    }
}
