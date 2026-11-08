namespace MyHomeRamen.Blazor.Features.Menu.Products.Shared;

public static class ProductStatusCodes
{
    public const string Active = "Active";
    public const string Retired = "Retired";
}

public sealed record ProductAllowedActions(bool CanCreate, bool CanEdit, bool CanRetire);

public sealed record ProductForManageResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record GetProductsForManageResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IEnumerable<ProductForManageResponse> Products,
    ProductAllowedActions AllowedActions);

public sealed record GetProductByIdForManageResponse(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds,
    string Status);

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record ProductMutationResponse(Guid Id);
