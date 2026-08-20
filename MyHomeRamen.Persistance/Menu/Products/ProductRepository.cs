using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class ProductRepository(MenuDbContext menuDbContext, ICacheService cacheService) : BaseRepository<Product, ProductId>(menuDbContext, cacheService), IProductRepository
{
    IProductQuery IProductRepository.Query() => this;

    IProductLoader IProductRepository.Load() => this;
}
