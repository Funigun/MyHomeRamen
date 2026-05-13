namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;

public sealed record GetProductsForManageRequest(
    string? Name,
    IEnumerable<Guid>? CategoryIds,
    IEnumerable<Guid>? IngredientIds,
    decimal? PriceFrom,
    decimal? PriceTo,
    string? OrderBy);
