namespace MyHomeRamen.Features.Menu.ExternalApi;

public sealed record MenuIngredientResult(Guid Id, string Name, string Description, decimal Price);

public sealed record MenuProductResult(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IReadOnlyList<MenuIngredientResult> BaseIngredients,
    IReadOnlyList<MenuIngredientResult> CustomIngredients);
