using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuCardRepository : IMenuCardLoader
{
    public async Task<IReadOnlyCollection<MenuCard>> ByRestaurant(
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .Where(card => card.RestaurantId == restaurantId)
            .ToListAsync(cancellationToken);

    public async Task<MenuCard?> ByIdForActivation(
        MenuCardId menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .FirstOrDefaultAsync(
                card => card.Id == menuCardId && card.RestaurantId == restaurantId,
                cancellationToken);

    public async Task<MenuCard?> ById(MenuCardId menuCardId, CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .Include(card => card.Sections)
                .ThenInclude(section => section.Items)
                    .ThenInclude(item => item.RestaurantProductOffer)
            .FirstOrDefaultAsync(card => card.Id == menuCardId, cancellationToken);
}
