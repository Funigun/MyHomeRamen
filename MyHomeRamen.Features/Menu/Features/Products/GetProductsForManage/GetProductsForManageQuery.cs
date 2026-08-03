using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

public sealed record GetProductsForManageQuery(PageParameters PageParameters, GetProductsForManageRequest Request) : IQuery<GetProductsForManageResponse>;

public sealed class GetProductsForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductsForManageQuery, GetProductsForManageResponse>
{
    public async Task<GetProductsForManageResponse> Handle(
        GetProductsForManageQuery query,
        CancellationToken cancellationToken)
    {
        ProductForManageFilter filter = new
        (
            query.Request.Name,
            query.Request.CategoryIds,
            query.Request.IngredientIds,
            query.Request.PriceFrom,
            query.Request.PriceTo
        );

        OrderParameters orderParameters = new(query.Request.OrderBy ?? "Name");

        PagedResult<ProductForManageDto> pagedResult = await dbContext.Product.Query().ForManage(filter, query.PageParameters, orderParameters, p => p.ToResponse(), cancellationToken);

        return new GetProductsForManageResponse(
            Page: query.PageParameters.PageNumber,
            PageSize: query.PageParameters.PageSize,
            TotalCount: pagedResult.TotalItems,
            Products: pagedResult.Items);
    }
}

