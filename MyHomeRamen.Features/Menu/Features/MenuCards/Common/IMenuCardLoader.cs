using MyHomeRamen.Domain.Menu.MenuCards;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.Common;

public interface IMenuCardLoader
{
    Task<IReadOnlyCollection<MenuCard>> ByRestaurant(Guid restaurantId, CancellationToken cancellationToken);

    Task<MenuCard?> ById(MenuCardId menuCardId, CancellationToken cancellationToken);

    Task<MenuCard?> ByIdForActivation(MenuCardId menuCardId, Guid restaurantId, CancellationToken cancellationToken);
}
