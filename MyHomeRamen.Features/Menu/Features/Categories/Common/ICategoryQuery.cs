using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public interface ICategoryQuery
{
    Task<List<Category>> GetByType(CategoryType categoryType, CancellationToken cancellationToken = default);

    Task<int> GetNextSortOrder(CategoryType categoryType, CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> GetByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default);

    Task<bool> Exists(CategoryId categoryId, CancellationToken cancellationToken = default);

    Task<bool> IsCategoryNameUnique(string name, CancellationToken cancellationToken = default);

    Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken = default);

    Task<bool> IsUsedByProducts(CategoryId categoryId, CancellationToken cancellationToken = default);

    Task<bool> IsUsedByIngredients(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<Category?> ById(CategoryId id, CancellationToken cancellationToken);
}
