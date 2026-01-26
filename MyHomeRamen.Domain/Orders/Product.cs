using System.Collections.ObjectModel;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    private readonly Collection<Ingredient> _baseIngredients = [];
    private readonly Collection<Ingredient> _customIngredients = [];

    public ProductId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public IReadOnlyList<Ingredient> BaseIngredients => _baseIngredients.ToList();

    public IReadOnlyList<Ingredient> CustomIngredients => _customIngredients.ToList();

    private Product()
    {
    }

    private Product(ProductId id, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients)
    {
        Id = id;
        _baseIngredients = baseIngredients;
        _customIngredients = customIngredients;
    }

    public static Product Create(ProductId id, string name, string description, decimal price, string imageUrl, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients)
    {
        return new Product(id, baseIngredients, customIngredients)
        {
            Name = name,
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };
    }
}
