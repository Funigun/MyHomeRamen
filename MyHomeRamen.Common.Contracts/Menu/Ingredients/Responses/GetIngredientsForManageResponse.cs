using MyHomeRamen.Common.Contracts.Menu.Ingredients.DTOs;

namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

public sealed record GetIngredientsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<IngredientForManageDto> Ingredients);
