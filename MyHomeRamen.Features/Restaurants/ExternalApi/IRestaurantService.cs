namespace MyHomeRamen.Features.Restaurants.ExternalApi;

public interface IRestaurantService
{
    Task<RestaurantInfo?> GetRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken);
}

public sealed record RestaurantInfo(Guid Id, Guid CompanyId, bool IsActive);
