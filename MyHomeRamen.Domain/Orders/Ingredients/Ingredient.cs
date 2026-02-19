using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Orders.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public IngredientId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal OriginalPrice { get; private set; }

    public decimal CalculatedPrice { get; private set; }

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, Guid restaurantId, IngredientId originalId)
    {
        Id = id;
        RestaurantId = restaurantId;
        OriginalId = originalId;
    }

    public static Ingredient Create(IngredientId id, Guid restaurantId, IngredientId originalId, string name, decimal price)
    {
        Ingredient ingredient = new(id, restaurantId, originalId)
        {
            Name = name,
            OriginalPrice = price
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }
}
