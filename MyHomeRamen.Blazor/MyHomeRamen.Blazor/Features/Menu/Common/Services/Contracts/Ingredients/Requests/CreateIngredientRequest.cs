namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Requests;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
