using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetMenuCategoriesQuery, IEnumerable<GetMenuCategoriesResponse>>
{
    public async Task<IEnumerable<GetMenuCategoriesResponse>> Handle(GetMenuCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
                              .ForCategoryType(CategoryType.Product)
                              .Select(c => c.ToMenuResponse())
                              .ToListAsync(cancellationToken);
    }
}
