using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public interface ICategorySpecification
{
    Task<IEnumerable<Category>> GetRemainingForResequencing(CategoryType categoryType, CategoryId excludeId, CancellationToken cancellationToken);

    Task<Category> ById(CategoryId categoryId, CancellationToken cancellationToken);

    Task<IEnumerable<Category>> ByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken);
}
