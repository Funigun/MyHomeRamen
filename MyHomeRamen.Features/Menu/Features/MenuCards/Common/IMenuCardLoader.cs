using MyHomeRamen.Domain.Menu.MenuCards;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.Common;

public interface IMenuCardLoader
{
    Task<MenuCard?> ById(MenuCardId menuCardId, CancellationToken cancellationToken);
}
