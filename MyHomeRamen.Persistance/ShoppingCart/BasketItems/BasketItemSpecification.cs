using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Features.ShoppingCart.Features.BasketItems.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public partial class ShoppingCartDbContext : IBasketItemSpecification
{
    public async Task<BasketItem?> ByIdAsync(BasketItemId basketItemId, CancellationToken cancellationToken)
        => await BasketItems.FirstOrDefaultAsync(item => item.Id == basketItemId, cancellationToken);
}
