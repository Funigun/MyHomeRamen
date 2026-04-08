namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;

public sealed record GetProductsByCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<ProductIngredientDto> Ingredients);
