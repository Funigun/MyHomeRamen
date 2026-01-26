using System.Collections.ObjectModel;
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private readonly Collection<Category> _categories = [];

    public IngredientId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, Collection<Category> categories)
    {
        Id = id;
        _categories = categories;
    }

    public static Ingredient Create(IngredientId id, string name, string description, decimal price, Collection<Category> categories)
    {
        return new(id, categories)
        {
            Name = name,
            Description = description,
            Price = price
        };
    }
}
