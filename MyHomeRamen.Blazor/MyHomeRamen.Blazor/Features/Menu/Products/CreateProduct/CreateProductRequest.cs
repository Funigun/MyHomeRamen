namespace MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds);
