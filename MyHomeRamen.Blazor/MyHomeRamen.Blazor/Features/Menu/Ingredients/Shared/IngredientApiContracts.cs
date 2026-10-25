namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

public sealed record GetIngredientsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<IngredientForManageResponse> Ingredients,
    IngredientAllowedActions AllowedActions);

public sealed record IngredientForManageResponse(Guid Id, string Name, string Description);

public sealed record IngredientAllowedActions(bool CanCreate, bool CanEdit, bool CanDelete);

public sealed record GetIngredientsForDropdownResponse(IEnumerable<IngredientForDropdownResponse> Ingredients);

public sealed record IngredientForDropdownResponse(Guid Id, string Name);

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);

public sealed record CreateIngredientRequest(string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds);

public sealed record UpdateIngredientRequest(string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds);
