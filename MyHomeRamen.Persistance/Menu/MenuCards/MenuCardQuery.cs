using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.MenuCards;
using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardForManage;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuCardRepository : IMenuCardQuery
{
    public async Task<PagedResult<MenuCardForManageDto>> ForManage(
        GetMenuCardsForManageQueryOptions options,
        CancellationToken cancellationToken)
        => await QueryPaged(
            menuDbContext.MenuCards
                .AsNoTracking()
                .Where(card => card.RestaurantId == options.RestaurantId),
            options,
            cancellationToken);

    public async Task<MenuCard?> ByIdForManage(
        MenuCardId menuCardId,
        Guid restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .AsNoTracking()
            .Where(card => card.Id == menuCardId && card.RestaurantId == restaurantId)
            .Include(card => card.Sections.OrderBy(section => section.SortOrder))
                .ThenInclude(section => section.Items.OrderBy(item => item.SortOrder))
                    .ThenInclude(item => item.RestaurantProductOffer)
                        .ThenInclude(offer => offer.Product)
                            .ThenInclude(product => product.BaseIngredients)
            .Include(card => card.Sections.OrderBy(section => section.SortOrder))
                .ThenInclude(section => section.Items.OrderBy(item => item.SortOrder))
                    .ThenInclude(item => item.RestaurantProductOffer)
                        .ThenInclude(offer => offer.Product)
                            .ThenInclude(product => product.CustomIngredients)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<MenuCardOptionRow>> Options(
        Guid? restaurantId,
        CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(card => card.IsActive && (!restaurantId.HasValue || card.RestaurantId == restaurantId.Value))
            .OrderBy(card => card.Name)
            .Select(card => new MenuCardOptionRow(card.RestaurantId, card.Id.Value, card.Name))
            .ToListAsync(cancellationToken);

    public async Task<MenuCard?> ActiveById(MenuCardId menuCardId, CancellationToken cancellationToken)
        => await menuDbContext.MenuCards
            .AsNoTracking()
            .Where(card => card.Id == menuCardId && card.IsActive)
            .Include(card => card.Sections.OrderBy(section => section.SortOrder))
                .ThenInclude(section => section.Items
                    .Where(item => item.RestaurantProductOffer.Status == RestaurantProductOfferStatus.Active
                        && item.RestaurantProductOffer.Product.Status == ProductStatus.Active)
                    .OrderBy(item => item.SortOrder))
                    .ThenInclude(item => item.RestaurantProductOffer)
                        .ThenInclude(offer => offer.Product)
                            .ThenInclude(product => product.BaseIngredients)
            .Include(card => card.Sections.OrderBy(section => section.SortOrder))
                .ThenInclude(section => section.Items
                    .Where(item => item.RestaurantProductOffer.Status == RestaurantProductOfferStatus.Active
                        && item.RestaurantProductOffer.Product.Status == ProductStatus.Active)
                    .OrderBy(item => item.SortOrder))
                    .ThenInclude(item => item.RestaurantProductOffer)
                        .ThenInclude(offer => offer.Product)
                            .ThenInclude(product => product.CustomIngredients)
            .FirstOrDefaultAsync(cancellationToken);
}
