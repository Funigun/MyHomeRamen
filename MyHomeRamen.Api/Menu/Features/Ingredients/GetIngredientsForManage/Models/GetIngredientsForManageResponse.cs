namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;

public sealed record GetIngredientsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<IngredientDto> Ingredients);
