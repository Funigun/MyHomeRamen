using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(IMenuDbContext dbContext)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        ProductId productId = query.Id;

        Product product = await dbContext.Products
                                         .Include(p => p.BaseIngredients)
                                         .Include(p => p.CustomIngredients)
                                         .AsSplitQuery()
                                         .GetByIdQuery(productId, cancellationToken);

        return product.ToResponse();
    }
}
