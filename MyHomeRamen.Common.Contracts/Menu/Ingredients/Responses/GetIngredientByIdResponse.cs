namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
