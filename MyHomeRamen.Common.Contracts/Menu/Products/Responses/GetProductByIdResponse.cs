using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;

namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;

public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);
