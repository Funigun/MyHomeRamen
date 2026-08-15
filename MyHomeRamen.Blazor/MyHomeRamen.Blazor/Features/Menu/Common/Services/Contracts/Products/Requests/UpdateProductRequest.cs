namespace MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Products.Requests;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);
