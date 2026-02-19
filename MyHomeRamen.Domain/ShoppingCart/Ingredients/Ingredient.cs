using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.ShoppingCart.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    public IngredientId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public IngredientId OriginalId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, Guid restaurantId, IngredientId originalId)
    {
        Id = id;
        RestaurantId = restaurantId;
        OriginalId = originalId;
    }

    public static Ingredient Create(IngredientId id, Guid restaurantId, IngredientId originalId, string name, string description, decimal price)
    {
        Ingredient ingredient = new(id, restaurantId, originalId)
        {
            Name = name,
            Description = description,
            Price = price
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }
}
