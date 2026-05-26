using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed class GetProductsForManageHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetProductsForManageQuery, GetProductsForManageResponse>
{
    public async Task<GetProductsForManageResponse> Handle(
        GetProductsForManageQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> productsQuery = dbContext.Products.ForManage(
            query.Request.Name,
            query.Request.CategoryIds,
            query.Request.IngredientIds,
            query.Request.PriceFrom,
            query.Request.PriceTo);

        int totalCount = await productsQuery.CountAsync(cancellationToken);

        productsQuery = string.Equals(query.Request.OrderBy, "Price", StringComparison.OrdinalIgnoreCase)
            ? productsQuery.OrderBy(p => p.Price)
            : productsQuery.OrderBy(p => p.Name);

        productsQuery = productsQuery.Paged(query.PageParameters.PageNumber, query.PageParameters.PageSize);

        List<ProductForManageDto> products = await productsQuery.Select(p => p.ToResponse()).ToListAsync(cancellationToken);

        return new GetProductsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: totalCount,
            Products: products);
    }
}
