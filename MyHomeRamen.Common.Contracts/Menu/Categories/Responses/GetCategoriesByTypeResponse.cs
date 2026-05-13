namespace MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

public sealed record GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder);
