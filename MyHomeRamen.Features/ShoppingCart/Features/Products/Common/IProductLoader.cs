using MyHomeRamen.Domain.ShoppingCart.Products;

namespace MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

public interface IProductLoader
{
    Task<Product?> ByIdAsync(ProductId productId, CancellationToken cancellationToken);
}
