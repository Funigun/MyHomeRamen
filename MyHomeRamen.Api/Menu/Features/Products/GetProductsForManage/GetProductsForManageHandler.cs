using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed class GetProductsForManageHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetProductsForManageRequest, GetProductsForManageResponse>
{
    public async Task<GetProductsForManageResponse> Handle(
        GetProductsForManageRequest request,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> query = dbContext.Products.ForManage(
            request.Name,
            request.CategoryIds,
            request.IngredientIds,
            request.PriceFrom,
            request.PriceTo);

        int totalCount = await query.CountAsync(cancellationToken);

        query = string.Equals(request.OrderBy, "Price", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(p => p.Price)
            : query.OrderBy(p => p.Name);

        query = query.Paged(request.PageParameters.PageNumber, request.PageParameters.PageSize);

        List<ProductDto> products = await query.Select(p => p.ToResponse()).ToListAsync(cancellationToken);

        return new GetProductsForManageResponse(
            Page: request.PageParameters.PageNumber,
            PageSize: request.PageParameters.PageSize,
            TotalCount: totalCount,
            Products: products);
    }
}
