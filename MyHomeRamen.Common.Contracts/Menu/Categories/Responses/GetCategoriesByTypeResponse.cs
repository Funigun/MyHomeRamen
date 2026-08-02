namespace MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

public sealed record CategoryByTypeDto(Guid Id, string Name, int SortOrder);

public sealed record GetCategoriesByTypeResponse(IEnumerable<CategoryByTypeDto> Categories);
