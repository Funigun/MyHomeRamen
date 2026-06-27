using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Orders.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; private set; }

    public IngredientId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal OriginalPrice { get; private set; }

    public decimal CalculatedPrice { get; private set; }

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, IngredientId originalId)
    {
        Id = id;
        OriginalId = originalId;
    }

    public static Ingredient Create(IngredientId id, IngredientId originalId, string name, decimal price)
    {
        Ingredient ingredient = new(id, originalId)
        {
            Name = name,
            OriginalPrice = price
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }
}
