using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductByIdForManage;

public record GetProductByIdForManageQuery(Guid Id) : IQuery<GetProductByIdForManageResponse>;

public sealed class GetProductByIdForManageHandler(IMenuDbContext dbContext) : IQueryHandler<GetProductByIdForManageQuery, GetProductByIdForManageResponse>
{
    public async Task<GetProductByIdForManageResponse> Handle(GetProductByIdForManageQuery query, CancellationToken cancellationToken)
    {
        ProductId productId = query.Id;

        Product product = await dbContext.Product.Query().ById(productId, cancellationToken);

        return product.ToResponse();
    }
}
