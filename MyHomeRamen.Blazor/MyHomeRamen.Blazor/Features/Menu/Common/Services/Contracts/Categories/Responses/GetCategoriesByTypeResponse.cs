namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Responses;

public sealed record CategoryByTypeDto(Guid Id, string Name, int SortOrder);

public sealed record GetCategoriesByTypeResponse(IEnumerable<CategoryByTypeDto> Categories);
