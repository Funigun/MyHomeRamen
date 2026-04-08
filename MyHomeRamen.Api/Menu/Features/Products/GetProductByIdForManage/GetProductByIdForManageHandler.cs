using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public sealed class GetProductByIdForManageHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetProductByIdForManageRequest, GetProductByIdForManageResponse>
{
    public async Task<GetProductByIdForManageResponse> Handle(GetProductByIdForManageRequest request, CancellationToken cancellationToken)
    {
        ProductId productId = request.Id;

        Product product = await dbContext.Products
            .Include(p => p.Categories)
            .Include(p => p.BaseIngredients)
            .GetBySelectorNotTrackedAsync(productId, cancellationToken);

        return product.ToResponse();
    }
}
