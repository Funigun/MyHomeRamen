namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Responses;

public sealed record CategoryByTypeDto(Guid Id, string Name, int SortOrder);

public sealed record CategoryAllowedActions(bool CanCreate, bool CanEdit, bool CanDelete);

public sealed record GetCategoriesByTypeResponse(
    IEnumerable<CategoryByTypeDto> Categories,
    CategoryAllowedActions AllowedActions);
