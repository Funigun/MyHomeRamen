using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductLoader
{
    Task<Product> ById(ProductId productId, CancellationToken cancellationToken);
}
