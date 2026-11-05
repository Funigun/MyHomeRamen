using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetEmployeeRestaurantOptions;
using Microsoft.EntityFrameworkCore;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class RestaurantRepository : IRestaurantQuery
{
    public async Task<bool> IsNameUnique(string name, CancellationToken cancellationToken)
        => !await Exists(r => r.Name == name, cancellationToken);

    public async Task<IEnumerable<EmployeeRestaurantOptionDto>> EmployeeOptions(
        IReadOnlyCollection<Guid> restaurantIds,
        CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking()
            .Where(restaurant => restaurant.IsActive && restaurantIds.Contains(restaurant.Id.Value))
            .OrderBy(restaurant => restaurant.Name)
            .Select(restaurant => new EmployeeRestaurantOptionDto(
                restaurant.Id.Value,
                restaurant.Name,
                restaurant.Address.Street,
                restaurant.Address.City,
                restaurant.Address.ZipCode,
                restaurant.Address.Location.Latitude,
                restaurant.Address.Location.Longitude))
            .ToListAsync(cancellationToken);
}
