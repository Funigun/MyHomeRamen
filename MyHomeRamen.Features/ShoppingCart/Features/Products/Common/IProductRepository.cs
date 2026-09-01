using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.ShoppingCart.Features.Products.Common;

public interface IProductRepository : IRepository<Product, ProductId>
{
    IProductQuery Query();

    IProductLoader Load();
}
