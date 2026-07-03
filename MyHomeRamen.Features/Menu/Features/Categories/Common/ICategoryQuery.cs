using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

public interface ICategoryQuery
{


    Task<bool> IsNameUnique(string name, CancellationToken cancellationToken = default);

    Task<bool> IsProductCategoryType(CategoryId categoryId, CancellationToken cancellationToken = default);
}
