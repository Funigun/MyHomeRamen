namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Requests;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);
