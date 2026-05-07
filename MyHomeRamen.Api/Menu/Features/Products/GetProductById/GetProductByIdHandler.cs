using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductById.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetProductByIdRequest, GetProductByIdResponse>
{
    public async Task<GetProductByIdResponse> Handle(GetProductByIdRequest request, CancellationToken cancellationToken)
    {
        ProductId productId = request.Id;

        Product product = await dbContext.Products
                                         .Include(p => p.BaseIngredients)
                                         .Include(p => p.CustomIngredients)
                                         .AsSplitQuery()
                                         .GetByIdQuery(productId, cancellationToken);

        return product.ToResponse();
    }
}
