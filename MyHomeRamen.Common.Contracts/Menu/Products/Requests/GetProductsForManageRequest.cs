namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;

public sealed record GetProductsForManageRequest(
    string? Name,
    Guid[]? CategoryIds,
    Guid[]? IngredientIds,
    decimal? PriceFrom,
    decimal? PriceTo,
    string? OrderBy);
