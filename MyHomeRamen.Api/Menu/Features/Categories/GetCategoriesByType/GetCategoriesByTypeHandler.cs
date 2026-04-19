using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.Caching;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Policies;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext, ICacheService cacheService)
                  : IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(GetCategoriesByTypeRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesByTypeResponse> categories = await cacheService.GetOrSetAsync(
            new GetCategoriesByTypeCachePolicy(),
            request,
            async ct => await GetCategoriesAsync(request, ct),
            cancellationToken);

        return categories;
    }

    private async Task<List<GetCategoriesByTypeResponse>> GetCategoriesAsync(GetCategoriesByTypeRequest request, CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        return await dbContext.Categories
                              .ForCategoryType(categoryType)
                              .Select(c => c.ToResponse())
                              .ToListAsync(cancellationToken);
    }
}
