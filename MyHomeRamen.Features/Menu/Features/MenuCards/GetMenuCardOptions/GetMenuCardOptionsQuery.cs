using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.Features.MenuCards.Common;
using MyHomeRamen.Features.Restaurants.ExternalApi;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardOptions;

public sealed record MenuCardOptionDto(Guid CompanyId, Guid RestaurantId, Guid MenuCardId, string Name);

public sealed record GetMenuCardOptionsQuery(Guid? RestaurantId) : IQuery<GetMenuCardOptionsResponse>;

public sealed class GetMenuCardOptionsAuthorizationPolicy(ICurrentUser currentUser)
                  : IAuthorizationPolicy<GetMenuCardOptionsQuery>
{
    public Task<bool> Authorize(GetMenuCardOptionsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(currentUser.HasAnyPermission("Menu", MenuPermissionConstants.CanViewMenuCard));
}

public sealed class GetMenuCardOptionsHandler(IMenuCardRepository menuCardRepository, IRestaurantService restaurantService, ICurrentUser currentUser)
                  : IRequestHandler<GetMenuCardOptionsQuery, GetMenuCardOptionsResponse>
{
    public async Task<GetMenuCardOptionsResponse> Handle(GetMenuCardOptionsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<MenuCardOptionRow> rows = await menuCardRepository.Query()
                                                                              .Options(query.RestaurantId, cancellationToken);

        Dictionary<Guid, RestaurantInfo?> restaurants = [];
        List<MenuCardOptionDto> options = [];

        foreach (MenuCardOptionRow row in rows)
        {
            if (!currentUser.HasPermission("Menu", MenuPermissionConstants.CanViewMenuCard, row.RestaurantId))
            {
                continue;
            }

            if (!restaurants.TryGetValue(row.RestaurantId, out RestaurantInfo? restaurant))
            {
                restaurant = await restaurantService.GetRestaurantAsync(row.RestaurantId, cancellationToken);
                restaurants.Add(row.RestaurantId, restaurant);
            }

            if (restaurant is { IsActive: true })
            {
                options.Add(new MenuCardOptionDto(
                    restaurant.CompanyId,
                    row.RestaurantId,
                    row.MenuCardId,
                    row.Name));
            }
        }

        return new GetMenuCardOptionsResponse(options);
    }
}
