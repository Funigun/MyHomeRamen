using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    private readonly List<Ingredient> _baseIngredients = [];
    private readonly List<Ingredient> _customIngredients = [];

    public ProductId Id { get; private set; }

    public ProductId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public string ImageUrl { get; private set; } = string.Empty;

    public IReadOnlyList<Ingredient> BaseIngredients => _baseIngredients.ToList();

    public IReadOnlyList<Ingredient> CustomIngredients => _customIngredients.ToList();

    private Product()
    {
    }

    private Product(ProductId id, ProductId originalId, List<Ingredient> baseIngredients, List<Ingredient> customIngredients)
    {
        Id = id;
        OriginalId = originalId;
        _baseIngredients = baseIngredients;
        _customIngredients = customIngredients;
    }

    public static Product Create(ProductId id, ProductId originalId, string description, decimal price, string imageUrl, List<Ingredient> baseIngredients, List<Ingredient> customIngredients)
    {
        return new Product(id, originalId, baseIngredients, customIngredients)
        {
            Description = description,
            Price = price,
            ImageUrl = imageUrl
        };
    }
}
