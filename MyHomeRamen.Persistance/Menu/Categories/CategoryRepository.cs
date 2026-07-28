using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class CategoryRepository(MenuDbContext menuDbContext) : BaseRepository<Category, CategoryId>(menuDbContext), ICategoryRepository
{
    public ICategoryQuery Query() => this;

    public ICategorySpecification Specification() => this;
}
