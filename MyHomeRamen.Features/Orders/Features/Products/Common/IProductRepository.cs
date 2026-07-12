using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Orders.Features.Products.Common;

public interface IProductRepository : IRepository<Product, ProductId>
{
    IProductQuery Query();

    IProductSpecification Specification();
}
