using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ProductRepository : IProductSpecification
{
    async Task<Product?> IProductSpecification.ByIdAsync(ProductId productId, CancellationToken cancellationToken)
        => await shoppingCartDbContext.Products.FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);
}
