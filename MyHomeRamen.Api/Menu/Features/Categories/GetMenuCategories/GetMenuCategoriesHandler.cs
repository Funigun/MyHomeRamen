using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext)
                  : IRequestHandler<GetMenuCategoriesRequest, IEnumerable<GetMenuCategoriesResponse>>
{
    public async Task<IEnumerable<GetMenuCategoriesResponse>> Handle(GetMenuCategoriesRequest request, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
                              .ForCategoryType(CategoryType.Product)
                              .Select(c => c.ToMenuResponse())
                              .ToListAsync(cancellationToken);
    }
}
