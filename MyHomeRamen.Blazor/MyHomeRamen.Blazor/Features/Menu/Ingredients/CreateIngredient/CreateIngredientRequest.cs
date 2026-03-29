namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.CreateIngredient;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
