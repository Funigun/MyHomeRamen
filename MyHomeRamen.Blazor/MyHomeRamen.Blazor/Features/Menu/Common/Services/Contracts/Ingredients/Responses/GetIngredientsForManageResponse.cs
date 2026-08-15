using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.DTOs;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Responses;

public sealed record GetIngredientsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<IngredientForManageDto> Ingredients);
