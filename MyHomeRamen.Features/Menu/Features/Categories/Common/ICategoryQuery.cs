using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public interface ICategoryQuery
{
    Task<List<Category>> GetByType(CategoryType categoryType, CancellationToken cancellationToken);

    Task<int> GetNextSortOrder(CategoryType categoryType, CancellationToken cancellationToken);

    Task<IEnumerable<Category>> GetByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken);

    Task<bool> Exists(CategoryId categoryId, CancellationToken cancellationToken);

    Task<bool> IsCategoryNameUnique(string name, CancellationToken cancellationToken);

    Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken);

    Task<bool> IsUsedByProducts(CategoryId categoryId, CancellationToken cancellationToken);

    Task<bool> IsUsedByIngredients(CategoryId categoryId, CancellationToken cancellationToken);
    Task<Category?> ById(CategoryId id, CancellationToken cancellationToken);
}
