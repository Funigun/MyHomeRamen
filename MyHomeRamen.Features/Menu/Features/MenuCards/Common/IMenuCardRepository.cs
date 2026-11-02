using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.Common;

public interface IMenuCardRepository : IRepository<MenuCard, MenuCardId>
{
    IMenuCardQuery Query();

    IMenuCardLoader Load();
}
