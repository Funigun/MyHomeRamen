using System.Collections.ObjectModel;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    private readonly Collection<Ingredient> _baseIngredients = [];
    private readonly Collection<Ingredient> _customIngredients = [];
    private readonly Collection<Category> _categories = [];

    public ProductId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public IReadOnlyList<Ingredient> BaseIngredients => _baseIngredients.ToList();

    public IReadOnlyList<Ingredient> CustomIngredients => _customIngredients.ToList();

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Product()
    {
    }

    private Product(ProductId id, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        Id = id;
        _baseIngredients = baseIngredients;
        _customIngredients = customIngredients;
        _categories = categories;
    }

    public static Product Create(ProductId id, string description, decimal price, string imageUrl, Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Collection<Category> categories)
    {
        return new Product(id, baseIngredients, customIngredients, categories)
        {
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };
    }
}
