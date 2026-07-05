using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IProductQuery
{
    public async Task<Product?> ByIdAsync(ProductId productId, CancellationToken cancellationToken)
        => await Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
}
