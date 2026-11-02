using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

public sealed class RestaurantIngredientPrice : AuditableEntity, IEntity<RestaurantIngredientPriceId>
{
    public RestaurantIngredientPriceId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public IngredientId IngredientId { get; private set; }

    public decimal Price { get; private set; }

    public Ingredient Ingredient { get; private set; } = default!;

    private RestaurantIngredientPrice() { }

    private RestaurantIngredientPrice(
        RestaurantIngredientPriceId id,
        Guid restaurantId,
        Ingredient ingredient)
    {
        Id = id;
        RestaurantId = restaurantId;
        IngredientId = ingredient.Id;
        Ingredient = ingredient;
    }

    public static RestaurantIngredientPrice Create(
        RestaurantIngredientPriceId id,
        Guid restaurantId,
        Ingredient ingredient,
        decimal price)
    {
        RestaurantIngredientPrice ingredientPrice = new(id, restaurantId, ingredient)
        {
            Price = price
        };

        RestaurantIngredientPriceValidator.Validate(ingredientPrice);
        return ingredientPrice;
    }

    public void UpdatePrice(decimal price)
    {
        Price = price;
        RestaurantIngredientPriceValidator.Validate(this);
    }
}
