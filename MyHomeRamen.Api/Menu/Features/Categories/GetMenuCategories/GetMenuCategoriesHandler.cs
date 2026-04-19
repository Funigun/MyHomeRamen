using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.Caching;
using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

public sealed class GetMenuCategoriesHandler(IMenuDbContext dbContext, ICacheService cacheService)
                  : IRequestHandler<GetMenuCategoriesRequest, IEnumerable<GetMenuCategoriesResponse>>
{
    public async Task<IEnumerable<GetMenuCategoriesResponse>> Handle(GetMenuCategoriesRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<GetMenuCategoriesResponse> categories = await cacheService.GetOrSetAsync(
            new GetMenuCategoriesCachePolicy(),
            request,
            async ct => await GetCategoriesAsync(ct),
            cancellationToken);

        return categories;
    }

    private async Task<List<GetMenuCategoriesResponse>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
                              .ForCategoryType(CategoryType.Product)
                              .Select(c => c.ToMenuResponse())
                              .ToListAsync(cancellationToken);
    }
}
