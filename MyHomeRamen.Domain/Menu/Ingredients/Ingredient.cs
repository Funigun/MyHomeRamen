using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Domain.Menu.Ingredients;

public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
    private List<Category> _categories = [];

    public IngredientId Id { get; private set; }

    public Guid CompanyId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = default!;

    public IReadOnlyList<Category> Categories => _categories.ToList();

    private Ingredient()
    {
    }

    private Ingredient(IngredientId id, Guid companyId, IEnumerable<Category> categories)
    {
        Id = id;
        CompanyId = companyId;
        _categories = categories.ToList();
    }

    public static Ingredient Create(IngredientId id, Guid companyId, string name, string description, IEnumerable<Category> categories)
    {
        Ingredient ingredient = new(id, companyId, categories)
        {
            Name = name,
            Description = description
        };

        IngredientValidator.Validate(ingredient);

        return ingredient;
    }

    public void Update(string name, string description, IEnumerable<Category> categories)
    {
        Name = name;
        Description = description;
        _categories.Clear();

        _categories = categories.ToList();

        IngredientValidator.Validate(this);
    }
}
