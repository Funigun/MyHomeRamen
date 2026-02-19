using System.Collections.ObjectModel;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Domain.Menu.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private readonly Collection<Category> _categories = [];

    public IngredientId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, Guid restaurantId, Collection<Category> categories)
    {
        Id = id;
        RestaurantId = restaurantId;
        _categories = categories;
    }

    public static Ingredient Create(IngredientId id, Guid restaurantId, string name, string description, decimal price, Collection<Category> categories)
    {
        Ingredient ingredient = new(id, restaurantId, categories)
        {
            Name = name,
            Description = description,
            Price = price
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }
}
