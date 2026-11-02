using MyHomeRamen.Domain.Menu.MenuCards;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.Common;

public interface IMenuCardQuery
{
    Task<IReadOnlyCollection<MenuCardOptionRow>> Options(Guid? restaurantId, CancellationToken cancellationToken);

    Task<MenuCard?> ActiveById(MenuCardId menuCardId, CancellationToken cancellationToken);
}
