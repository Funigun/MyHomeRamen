using System.Linq.Expressions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public record GetCategoryByTypeQueryOptions(CategoryType CategoryType, Expression<Func<Category, CategoryByTypeDto>> Selector)
            : DbQueryOptions<Category, CategoryByTypeDto>
              (
                    new()
                    {
                        Filter = c => c.CategoryType == CategoryType,
                        Selector = Selector
                    }
              );
