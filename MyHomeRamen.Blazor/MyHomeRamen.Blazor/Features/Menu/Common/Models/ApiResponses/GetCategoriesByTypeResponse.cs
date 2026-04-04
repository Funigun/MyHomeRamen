namespace MyHomeRamen.Blazor.Features.Menu.Common.Models.ApiResponses;

public sealed record GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder);
