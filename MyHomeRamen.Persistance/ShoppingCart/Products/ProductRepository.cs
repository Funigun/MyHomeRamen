using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Products.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class ProductRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<Product, ProductId>(shoppingCartDbContext, cacheService), IProductRepository
{
    IProductQuery IProductRepository.Query() => this;

    IProductSpecification IProductRepository.Specification() => this;
}
