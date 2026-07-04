using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        ProductId productId = query.Id;

        Product product = await dbContext.Product.Query().ById(productId, cancellationToken);

        return product.ToResponse();
    }
}
