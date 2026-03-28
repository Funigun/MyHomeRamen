using MyHomeRamen.Blazor.Features.Menu.Products.CreateProduct;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }

    public IEnumerable<Guid> IngredientIds { get; set; } = [];

    public CreateProductRequest ToCreateRequest()
    {
        return new CreateProductRequest(
            Name,
            string.IsNullOrWhiteSpace(Description) ? null : Description,
            Price,
            CategoryId,
            IngredientIds);
    }
}
