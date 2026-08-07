using MyHomeRamen.Domain.Orders.Products;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Products.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class ProductRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Product, ProductId>(ordersDbContext, cacheService), IProductRepository
{
    public IProductQuery Query() => this;

    public IProductSpecification Specification() => this;
}