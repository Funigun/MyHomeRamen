using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Menu.Categories;

public class Category : AuditableEntity, IEntity<CategoryId>
{
    public CategoryId Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public CategoryType CategoryType { get; private set; }

    private Category()
    {

    }

    private Category(CategoryId id)
    {
        Id = id;
    }

    public static Category Create(CategoryId id, string name, int sortOrder, CategoryType categoryType)
    {
        Category category = new(id)
        {
            Name = name,
            SortOrder = sortOrder,
            CategoryType = categoryType
        };

        CategoryValidator.Validate(category);

        return category;
    }

    public void UpdateSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        CategoryValidator.ValidateSortOrder(this);
    }
}
