using MyHomeRamen.Blazor.Features.Menu.Products.Responses;

namespace MyHomeRamen.Blazor.Features.Menu.Products.Components;

public sealed class ProductTableModel
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public static ProductTableModel FromResponse(ProductForManageItemResponse response)
    {
        return new ProductTableModel
        {
            Id = response.Id,
            Name = response.Name,
            Description = response.Description ?? string.Empty,
            Price = response.Price,
        };
    }
}
