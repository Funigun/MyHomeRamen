using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Persistance.Common;
using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class CategoryRepository(MenuDbContext menuDbContext, ICacheService cacheService) : BaseRepository<Category, CategoryId>(menuDbContext, cacheService), ICategoryRepository
{
    public ICategoryQuery Query() => this;

    public ICategoryLoader Load() => this;
}
