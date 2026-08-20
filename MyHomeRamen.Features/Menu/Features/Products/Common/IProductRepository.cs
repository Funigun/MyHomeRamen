using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

public interface IProductRepository : IRepository<Product, ProductId>
{
    IProductQuery Query();

    IProductLoader Load();
}
