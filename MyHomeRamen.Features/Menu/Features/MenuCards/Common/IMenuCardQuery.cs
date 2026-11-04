using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.Common;

public interface IMenuCardQuery
{
    Task<PagedResult<MenuCardForManageDto>> ForManage(
        GetMenuCardsForManageQueryOptions options,
        CancellationToken cancellationToken);

    Task<MenuCard?> ByIdForManage(MenuCardId menuCardId, Guid restaurantId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MenuCardOptionRow>> Options(Guid? restaurantId, CancellationToken cancellationToken);

    Task<MenuCard?> ActiveById(MenuCardId menuCardId, CancellationToken cancellationToken);
}
