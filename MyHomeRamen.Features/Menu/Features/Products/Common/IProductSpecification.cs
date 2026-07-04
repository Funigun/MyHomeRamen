using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductSpecification
{
    Task<Product> ById(ProductId productId, CancellationToken cancellationToken);
}
