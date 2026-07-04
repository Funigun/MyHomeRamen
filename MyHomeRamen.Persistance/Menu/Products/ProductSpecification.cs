using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IProductSpecification
{
    async Task<Product> IProductSpecification.ById(ProductId productId, CancellationToken cancellationToken)
        => await Products.Include(p => p.Categories)
                         .Include(p => p.BaseIngredients)
                         .Include(p => p.CustomIngredients)
                         .AsSplitQuery()
                         .FirstAsync(p => p.Id == productId, cancellationToken);
}
