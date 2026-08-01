using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class CategoryRepository : ICategorySpecification
{
    async Task<Category> ICategorySpecification.ById(CategoryId categoryId, CancellationToken cancellationToken)
        => await First(c => c.Id == categoryId, cancellationToken);

    async Task<IEnumerable<Category>> ICategorySpecification.ByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<Category>() { Filter = c => categoryIds.Contains(c.Id) }, cancellationToken );

    async Task<IEnumerable<Category>> ICategorySpecification.GetRemainingForResequencing(CategoryType categoryType, CategoryId excludeId, CancellationToken cancellationToken)
        => await List(new DbQueryOptions<Category>() { Filter = c => c.CategoryType == categoryType && c.Id != excludeId }, cancellationToken );
}
