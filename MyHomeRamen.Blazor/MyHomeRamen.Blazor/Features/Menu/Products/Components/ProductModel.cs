using MyHomeRamen.Blazor.Features.Menu.Common.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public CategoryOption? CategoryId { get; set; }

    public IEnumerable<IngredientOption> IngredientIds { get; set; } = [];

    public IEnumerable<IngredientOption> CustomIngredientIds { get; set; } = [];

    public CreateProductRequest ToCreateRequest()
    {
        return new CreateProductRequest(
            Name,
            string.IsNullOrWhiteSpace(Description) ? null : Description,
            Price,
            CategoryId?.Id ?? Guid.Empty,
            IngredientIds.Select(i => i.Id),
            CustomIngredientIds.Select(i => i.Id));
    }

    public UpdateProductRequest ToEditRequest()
    {
        return new UpdateProductRequest(
            Name,
            string.IsNullOrWhiteSpace(Description) ? null : Description,
            Price,
            CategoryId?.Id ?? Guid.Empty,
            IngredientIds.Select(i => i.Id),
            CustomIngredientIds.Select(i => i.Id));
    }

    public static ProductModel FromResponse(
        GetProductByIdForManageResponse response,
        IEnumerable<CategoryOption> availableCategories,
        IEnumerable<IngredientOption> availableIngredients)
    {
        return new ProductModel
        {
            Name = response.Name,
            Description = response.Description,
            Price = response.Price,
            CategoryId = availableCategories.FirstOrDefault(c => c.Id == response.CategoryId),
            IngredientIds = availableIngredients.Where(i => response.IngredientIds.Contains(i.Id)),
            CustomIngredientIds = availableIngredients.Where(i => response.CustomIngredientIds.Contains(i.Id)),
        };
    }
}
