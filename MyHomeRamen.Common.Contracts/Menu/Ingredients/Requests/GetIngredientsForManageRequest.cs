namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;

public sealed record GetIngredientsForManageRequest(string? Name, IEnumerable<Guid>? CategoryIds);
