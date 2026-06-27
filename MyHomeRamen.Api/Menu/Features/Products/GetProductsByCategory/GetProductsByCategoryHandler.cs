using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryHandler(IMenuDbContext dbContext)
                  : IQueryHandler<GetProductsByCategoryQuery, IEnumerable<GetProductsByCategoryResponse>>
{
    public async Task<IEnumerable<GetProductsByCategoryResponse>> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Products
                              .ForCategory(new CategoryId(query.Request.CategoryId))
                              .Select(p => p.ToResponse())
                              .ToListAsync(cancellationToken);
    }
}
