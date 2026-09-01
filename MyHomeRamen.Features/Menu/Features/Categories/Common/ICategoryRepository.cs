using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    ICategoryQuery Query();

    ICategoryLoader Load();
}
