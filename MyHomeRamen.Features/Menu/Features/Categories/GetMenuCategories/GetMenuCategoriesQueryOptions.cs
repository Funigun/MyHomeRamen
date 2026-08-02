using System.Linq.Expressions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

public sealed record GetMenuCategoriesQueryOptions()
                   : DbQueryOptions<Category, CategoryForMenuDto>
                   (
                       new DbQueryOptions<Category, CategoryForMenuDto>
                       {
                           Selector = c => new(c.Id.Value, c.Name)
                       }
                   );

