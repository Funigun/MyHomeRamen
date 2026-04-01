namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;

public sealed record GetCategoriesForManageResponse(
    IEnumerable<CategoryForManageDto> ProductCategories,
    IEnumerable<CategoryForManageDto> IngredientCategories);

public sealed record CategoryForManageDto(Guid Id, string Name, int SortOrder);
