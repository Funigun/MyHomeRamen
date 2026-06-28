using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetProductByIdForManageQuery, GetProductByIdForManageResponse>
{
    public async Task<GetProductByIdForManageResponse> Handle(GetProductByIdForManageQuery query, CancellationToken cancellationToken)
    {
        ProductId productId = query.Id;

        Product product = await dbContext.Products
            .Include(p => p.Categories)
            .Include(p => p.BaseIngredients)
            .Include(p => p.CustomIngredients)
            .AsSplitQuery()
            .GetByIdQuery(productId, cancellationToken);

        return product.ToResponse();
    }
}
