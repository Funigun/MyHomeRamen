using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Domain.Menu.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private List<Category> _categories = [];

    public IngredientId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, IEnumerable<Category> categories)
    {
        Id = id;
        _categories = categories.ToList();
    }

    public static Ingredient Create(IngredientId id, string name, string description, decimal price, IEnumerable<Category> categories)
    {
        Ingredient ingredient = new(id, categories)
        {
            Name = name,
            Description = description,
            Price = price
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }

    public void Update(string name, string description, decimal price, IEnumerable<Category> categories)
    {
        Name = name;
        Description = description;
        Price = price;
        _categories.Clear();

        _categories = categories.ToList();

        IngredientValidator.Validate(this);
    }
}
