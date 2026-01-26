using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu;

public sealed class Category : AuditableEntity, IEntity<CategoryId>
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
        return new Category(id)
        {
            Name = name,
            SortOrder = sortOrder,
            CategoryType = categoryType
        };
    }
}
