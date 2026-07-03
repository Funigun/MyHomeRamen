using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : ICategoryQuery
{
    private IQueryable<Category> CategoriesQuery => Categories.AsNoTracking();

    public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken = default)
        => !await CategoriesQuery.AnyAsync(c => c.Name == name, cancellationToken);

    public async Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken = default)
        => await CategoriesQuery.AnyAsync(c => c.Id == categoryId && c.CategoryType == CategoryType.Product, cancellationToken);
}
