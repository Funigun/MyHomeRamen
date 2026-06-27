using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetCategoriesByTypeQuery, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(GetCategoriesByTypeQuery request, CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        return await dbContext.Categories
                              .ForCategoryType(categoryType)
                              .Select(c => c.ToResponse())
                              .ToListAsync(cancellationToken);
    }
}
