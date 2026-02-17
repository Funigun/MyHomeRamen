using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu.Categories;

public sealed class Category : AuditableEntity, IEntity<CategoryId>
{
    public CategoryId Id { get; private set; }

    public Guid RestaurantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public CategoryType CategoryType { get; private set; }

    private Category()
    {
    }

    private Category(CategoryId id, Guid restaurantId)
    {
        Id = id;
        RestaurantId = restaurantId;
    }

    public static Category Create(CategoryId id, Guid restaurantId, string name, int sortOrder, CategoryType categoryType)
    {
        Category category = new(id, restaurantId)
        {
            Name = name,
            SortOrder = sortOrder,
            CategoryType = categoryType
        };

        CategoryValidator.Validate(category);

        return category;
    }
}
