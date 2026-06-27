using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.ShoppingCart.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; private set; }

    public IngredientId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal Price { get; private set; }

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, IngredientId originalId)
    {
        Id = id;
        OriginalId = originalId;
    }

    public static Ingredient Create(IngredientId id, IngredientId originalId, string name, string description, decimal price, int quantity)
    {
        Ingredient ingredient = new(id, originalId)
        {
            Name = name,
            Description = description,
            Price = price,
            Quantity = quantity
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }
}
