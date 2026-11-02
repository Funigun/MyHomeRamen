using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuCardRepository : IMenuCardLoader
{
    public async Task<MenuCard?> ById(MenuCardId menuCardId, CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .Include(card => card.Sections)
                .ThenInclude(section => section.Items)
                    .ThenInclude(item => item.RestaurantProductOffer)
            .FirstOrDefaultAsync(card => card.Id == menuCardId, cancellationToken);
}
