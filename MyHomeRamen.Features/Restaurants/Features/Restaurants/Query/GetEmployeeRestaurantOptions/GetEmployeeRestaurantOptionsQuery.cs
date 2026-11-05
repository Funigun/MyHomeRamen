using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetEmployeeRestaurantOptions;

public sealed record GetEmployeeRestaurantOptionsQuery : IQuery<GetEmployeeRestaurantOptionsResponse>;

public sealed record EmployeeRestaurantOptionDto(
    Guid Id,
    string Name,
    string Street,
    string City,
    string ZipCode,
    double Latitude,
    double Longitude);

public sealed class GetEmployeeRestaurantOptionsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetEmployeeRestaurantOptionsQuery>
{
    public Task<bool> Authorize(GetEmployeeRestaurantOptionsQuery query, CancellationToken cancellationToken)
        => Task.FromResult(
            currentUser.IsAuthenticated
            && currentUser.PermissionGrants.Any(grant =>
                grant.Scope == RoleScope.Restaurant
                && grant.IsResourceScoped
                && grant.RestaurantId.HasValue));
}

public sealed class GetEmployeeRestaurantOptionsHandler(IRestaurantDbContext dbContext, ICurrentUser currentUser)
    : IRequestHandler<GetEmployeeRestaurantOptionsQuery, GetEmployeeRestaurantOptionsResponse>
{
    public async Task<GetEmployeeRestaurantOptionsResponse> Handle(GetEmployeeRestaurantOptionsQuery query, CancellationToken cancellationToken)
    {
        Guid[] restaurantIds = currentUser.PermissionGrants
            .Where(grant =>
                grant.Scope == RoleScope.Restaurant
                && grant.IsResourceScoped
                && grant.RestaurantId.HasValue)
            .Select(grant => grant.RestaurantId!.Value)
            .Distinct()
            .ToArray();

        IEnumerable<EmployeeRestaurantOptionDto> restaurants = await dbContext.Restaurant.Query()
            .EmployeeOptions(restaurantIds, cancellationToken);

        return new GetEmployeeRestaurantOptionsResponse(restaurants);
    }
}
