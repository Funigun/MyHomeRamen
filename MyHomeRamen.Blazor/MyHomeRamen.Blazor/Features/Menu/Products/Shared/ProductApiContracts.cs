namespace MyHomeRamen.Blazor.Features.Menu.Products.Shared;

public sealed record ProductAllowedActions(bool CanCreate, bool CanEdit, bool CanDelete);

public sealed record ProductForManageResponse(Guid Id, string Name, string? Description, decimal Price);

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
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record ProductMutationResponse(Guid Id);
