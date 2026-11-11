using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Common;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantOptions;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public sealed partial class RestaurantRepository(RestaurantsDbContext restaurantsDbContext, ICacheService cacheService) : BaseRepository<Restaurant, RestaurantId>(restaurantsDbContext, cacheService), IRestaurantRepository
{
    public IRestaurantQuery Query() => this;

    public IRestaurantLoader Load() => this;

    public async Task<RestaurantDetailsDto?> Details(RestaurantId restaurantId, CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking().Where(r => r.Id == restaurantId).Select(r => new RestaurantDetailsDto(r.Id.Value, r.Name, r.IsActive, r.Address.Street, r.Address.City, r.Address.ZipCode, r.Address.Location.Latitude, r.Address.Location.Longitude, r.ContactDetails == null ? null : r.ContactDetails.Phone, r.ContactDetails == null ? null : r.ContactDetails.Email, r.BankAccount == null ? null : r.BankAccount.AccountNumber, r.BankAccount == null ? null : r.BankAccount.BankName, r.BankAccount == null ? null : r.BankAccount.RoutingNumber)).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<RestaurantListItemDto>> Available(IReadOnlyCollection<Guid>? restaurantIds, CancellationToken cancellationToken)
    {
        IQueryable<Restaurant> query = restaurantsDbContext.Restaurants.AsNoTracking();

        if (restaurantIds is not null)
        {
            RestaurantId[] typedRestaurantIds = restaurantIds.Select(restaurantId => new RestaurantId(restaurantId)).ToArray();
            query = query.Where(restaurant => typedRestaurantIds.Contains(restaurant.Id));
        }

        return await query.OrderBy(restaurant => restaurant.CreatedOn)
                          .Select(restaurant => new RestaurantListItemDto(
                              restaurant.Id.Value,
                              restaurant.Name,
                              restaurant.IsActive,
                              restaurant.Address.Street,
                              restaurant.Address.City,
                              restaurant.Address.ZipCode,
                              restaurant.Address.Location.Latitude,
                              restaurant.Address.Location.Longitude,
                              restaurant.ContactDetails == null ? null : restaurant.ContactDetails.Phone,
                              restaurant.ContactDetails == null ? null : restaurant.ContactDetails.Email))
                          .ToListAsync(cancellationToken);
    }

    public async Task<RestaurantInfo?> ForExternalApi(Guid restaurantId, CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking()
            .Where(restaurant => restaurant.Id == new RestaurantId(restaurantId))
            .Select(restaurant => new RestaurantInfo(
                restaurant.Id.Value,
                EF.Property<CompanyId>(restaurant, "CompanyId").Value,
                restaurant.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<RestaurantOptionDto>> ActiveOptions(CancellationToken cancellationToken)
        => await restaurantsDbContext.Restaurants.AsNoTracking()
            .Where(restaurant => restaurant.IsActive)
            .OrderBy(restaurant => restaurant.Name)
            .Select(restaurant => new RestaurantOptionDto(
                restaurant.Id.Value,
                restaurant.Name,
                restaurant.Address.Street,
                restaurant.Address.City,
                restaurant.Address.ZipCode,
                restaurant.Address.Location.Latitude,
                restaurant.Address.Location.Longitude))
            .ToListAsync(cancellationToken);
}

