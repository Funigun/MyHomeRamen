using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class ProductRepository(MenuDbContext menuDbContext) : BaseRepository<Product, ProductId>(menuDbContext), IProductRepository
{
    IProductQuery IProductRepository.Query() => this;

    IProductSpecification IProductRepository.Specification() => this;
}
