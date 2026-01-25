using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    public ProductId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    private Product()
    {
    }

    public static Product Create(ProductId id, string name, string description, decimal price, string imageUrl)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };
    }
}
