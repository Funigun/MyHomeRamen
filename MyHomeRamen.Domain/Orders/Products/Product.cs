using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Orders.Ingredients;

namespace MyHomeRamen.Domain.Orders.Products;

public sealed class Product : AuditableEntity, IEntity<ProductId>
{
    private readonly List<Ingredient> _baseIngredients = [];
    private readonly List<Ingredient> _customIngredients = [];

    public ProductId Id { get; private set; }

    public ProductId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal OriginalPrice { get; private set; }

    public decimal CalculatedPrice { get; private set; }

    public bool Paid { get; private set; }

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

    public static Product Create(ProductId id, ProductId originalId, string name, decimal price, List<Ingredient> baseIngredients, List<Ingredient> customIngredients)
    {
        Product product = new(id, originalId, baseIngredients, customIngredients)
        {
            Name = name,
            OriginalPrice = price,
        };

        ProductValidator.Validate(product);

        return product;
    }

    public void MarkAsPaid()
    {
        Paid = true;
    }
}
