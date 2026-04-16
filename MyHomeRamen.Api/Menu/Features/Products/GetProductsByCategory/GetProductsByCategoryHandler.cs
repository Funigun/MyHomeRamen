using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Policies;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryHandler(IMenuDbContext dbContext, ICacheService cacheService)
                  : IRequestHandler<GetProductsByCategoryRequest, IEnumerable<GetProductsByCategoryResponse>>
{
    public async Task<IEnumerable<GetProductsByCategoryResponse>> Handle(
        GetProductsByCategoryRequest request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Product> products = await cacheService.GetOrSetAsync
        (
            new GetProductsByCategoryCachePolicy(),
            request,
            async ct => await GetProductsAsync(request, ct),
            cancellationToken
        );

        return products.Select(p => p.ToResponse());
    }

    private async Task<List<Product>> GetProductsAsync(GetProductsByCategoryRequest request, CancellationToken cancellationToken)
    {
        return await dbContext.Products
                              .ForCategory(new CategoryId(request.CategoryId))
                              .ToListAsync(cancellationToken);
    }
}
