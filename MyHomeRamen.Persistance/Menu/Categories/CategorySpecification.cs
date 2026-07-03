using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategorySpecification
{
    public Task<Category> ById(CategoryId categoryId, CancellationToken cancellationToken = default) => Categories.FirstAsync(c => c.Id == categoryId, cancellationToken);

    public async Task<List<Category>> GetRemainingForResequencing(CategoryType categoryType, CategoryId excludeId, CancellationToken cancellationToken = default)
        => await Categories.Where(c => c.CategoryType == categoryType && c.Id != excludeId)
                           .OrderBy(c => c.SortOrder)
                           .ToListAsync(cancellationToken);
}
