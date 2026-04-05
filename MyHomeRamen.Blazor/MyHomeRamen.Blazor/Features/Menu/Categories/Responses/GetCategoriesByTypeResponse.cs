namespace MyHomeRamen.Blazor.Features.Menu.Categories.Responses;

public sealed record GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder);
