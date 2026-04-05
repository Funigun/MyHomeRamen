namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Requests;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
