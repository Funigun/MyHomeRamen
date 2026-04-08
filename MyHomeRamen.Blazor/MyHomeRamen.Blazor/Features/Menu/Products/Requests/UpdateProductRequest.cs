namespace MyHomeRamen.Blazor.Features.Menu.Products.Requests;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds);
