namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
