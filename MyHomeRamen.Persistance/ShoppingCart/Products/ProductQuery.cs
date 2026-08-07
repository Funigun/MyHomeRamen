using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ProductRepository : IProductQuery
{
    public async Task<Product?> ByIdAsync(ProductId productId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
}
