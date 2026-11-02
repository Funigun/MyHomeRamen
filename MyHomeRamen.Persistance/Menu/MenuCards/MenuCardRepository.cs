using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class MenuCardRepository(MenuDbContext menuDbContext, ICacheService cacheService)
    : BaseRepository<MenuCard, MenuCardId>(menuDbContext, cacheService), IMenuCardRepository
{
    public IMenuCardQuery Query() => this;

    public IMenuCardLoader Load() => this;
}
