using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Responses;

public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);
