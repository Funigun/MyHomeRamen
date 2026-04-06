namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

public sealed record GetIngredientsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<IngredientForManageItemResponse> Ingredients);
