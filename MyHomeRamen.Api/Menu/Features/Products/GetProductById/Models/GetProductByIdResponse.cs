namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById.Models;

public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);
