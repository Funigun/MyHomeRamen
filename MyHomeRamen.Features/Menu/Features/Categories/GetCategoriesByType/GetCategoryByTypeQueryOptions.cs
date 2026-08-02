using System.Linq.Expressions;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public record GetCategoryByTypeQueryOptions(CategoryType CategoryType)
            : DbQueryOptions<Category, CategoryByTypeDto>
              (
                    new()
                    {
                        OrderBy = c => c.SortOrder,
                        OrderDirection = "asc",
                        Filter = c => c.CategoryType == CategoryType,
                        Selector = c => new CategoryByTypeDto(c.Id, c.Name, c.SortOrder)
                    }
              );
