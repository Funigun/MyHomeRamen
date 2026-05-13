namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;

public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
