using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Features.Companies.GetDetails;

namespace MyHomeRamen.Features.Restaurants.Services;

public sealed class RestaurantService(IRestaurantDbContext dbContext) : IRestaurantService
{
    public async Task<Guid?> GetCompanyIdAsync(CancellationToken cancellationToken)
        => (await dbContext.Company.Query().CurrentDetails(cancellationToken))?.Id;

    public async Task<RestaurantInfo?> GetRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken)
        => await dbContext.Restaurant.Query().ForExternalApi(restaurantId, cancellationToken);
}
