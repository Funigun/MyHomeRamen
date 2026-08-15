namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Ingredients.Requests;

public sealed record GetIngredientsForManageRequest(string? Name, Guid[]? CategoryIds);
