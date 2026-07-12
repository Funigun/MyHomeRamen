namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

public sealed record ProductForManageFilter
(
    string? Name, 
    IEnumerable<Guid>? CategoryIds,
    IEnumerable<Guid>? IngredientIds,
    decimal? PriceFrom,
    decimal? PriceTo
);
