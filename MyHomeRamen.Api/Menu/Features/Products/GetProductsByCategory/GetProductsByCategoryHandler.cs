using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryHandler(IMenuDbContext dbContext)
                  : IRequestHandler<GetProductsByCategoryRequest, IEnumerable<GetProductsByCategoryResponse>>
{
    public async Task<IEnumerable<GetProductsByCategoryResponse>> Handle(GetProductsByCategoryRequest request, CancellationToken cancellationToken)
    {
        return await dbContext.Products
                              .ForCategory(new CategoryId(request.CategoryId))
                              .Select(p => p.ToResponse())
                              .ToListAsync(cancellationToken);
    }
}
