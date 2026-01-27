using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Basket;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; private set; }

    public IngredientId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, IngredientId originalId)
    {
        Id = id;
        OriginalId = originalId;
    }

    public static Ingredient Create(IngredientId id, IngredientId originalId, string name, string description, decimal price)
    {
        return new Ingredient(id, originalId)
        {
            Name = name,
            Description = description,
            Price = price
        };
    }
}
