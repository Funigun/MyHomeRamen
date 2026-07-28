using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetMenuCategoriesQuery, IEnumerable<GetMenuCategoriesResponse>>
{
    public async Task<IEnumerable<GetMenuCategoriesResponse>> Handle(GetMenuCategoriesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Category.Query().GetByType(CategoryType.Product, cancellationToken);

        return categories.Select(c => c.ToMenuResponse());
    }
}
