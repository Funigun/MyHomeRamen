using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;

public sealed record GetProductsByCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<ProductIngredientDto> Ingredients);
