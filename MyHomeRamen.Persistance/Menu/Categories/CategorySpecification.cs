using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategorySpecification
{
    public async Task<Category> ById(CategoryId categoryId, CancellationToken cancellationToken) 
        => await Categories.FirstAsync(c => c.Id == categoryId, cancellationToken);

    public async Task<IEnumerable<Category>> ByIds(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken)
        => await Categories.Where(category => categoryIds.Contains(category.Id)).ToListAsync(cancellationToken);

    public async Task<List<Category>> GetRemainingForResequencing(CategoryType categoryType, CategoryId excludeId, CancellationToken cancellationToken)
        => await Categories.Where(c => c.CategoryType == categoryType && c.Id != excludeId)
                           .OrderBy(c => c.SortOrder)
                           .ToListAsync(cancellationToken);
}
