using MyHomeRamen.Blazor.Features.Menu.Products.Shared;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Models;

public sealed class ProductFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.5m;
    public Guid CategoryId { get; set; }
    public IReadOnlyCollection<Guid> IngredientIds { get; set; } = [];
    public IReadOnlyCollection<Guid> CustomIngredientIds { get; set; } = [];

    public CreateProductRequest ToCreateRequest() =>
        new(Name, string.IsNullOrWhiteSpace(Description) ? null : Description, Price, CategoryId, IngredientIds, CustomIngredientIds);

    public UpdateProductRequest ToUpdateRequest() =>
        new(Name, string.IsNullOrWhiteSpace(Description) ? null : Description, Price, CategoryId, IngredientIds, CustomIngredientIds);

    public static ProductFormModel FromResponse(GetProductByIdForManageResponse response) => new()
    {
        Name = response.Name,
        Description = response.Description,
        Price = response.Price,
        CategoryId = response.CategoryId,
        IngredientIds = response.IngredientIds.ToArray(),
        CustomIngredientIds = response.CustomIngredientIds.ToArray()
    };
}
