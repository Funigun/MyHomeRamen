using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetAvailableRestaurants;

public sealed record GetAvailableRestaurantsQuery : IQuery<GetAvailableRestaurantsResponse>;

public sealed record GetAvailableRestaurantsResponse(IEnumerable<RestaurantListItemDto> Restaurants);

public sealed class GetAvailableRestaurantsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetAvailableRestaurantsQuery>
{
    public async Task<bool> Authorize(GetAvailableRestaurantsQuery request, CancellationToken cancellationToken) 
        => await Task.FromResult(currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsManage));
}

public sealed class GetAvailableRestaurantsHandler(IRestaurantDbContext dbContext) : IRequestHandler<GetAvailableRestaurantsQuery, GetAvailableRestaurantsResponse>
{
    public async Task<GetAvailableRestaurantsResponse> Handle(GetAvailableRestaurantsQuery query, CancellationToken cancellationToken) 
        => new(await dbContext.Restaurant.Query().Available(cancellationToken));
}
