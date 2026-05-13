using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;

public sealed record GetProductsByCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<ProductIngredientDto> Ingredients);
