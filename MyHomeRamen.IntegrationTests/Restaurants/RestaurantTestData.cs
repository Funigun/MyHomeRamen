using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Persistance.Restaurants;

namespace MyHomeRamen.IntegrationTests.Restaurants;

public sealed class RestaurantTestData
{
    private DbContextOptions<RestaurantsDbContext>? _dbContextOptions;
    private ICurrentUser? _currentUser;
    private Guid _companyId;

    public Guid RestaurantId { get; private set; }

    public async Task<Guid> InitializeAsync(
        string connectionString,
        ICurrentUser currentUser,
        Guid companyId,
        CancellationToken cancellationToken)
    {
        if (_dbContextOptions is not null)
        {
            throw new InvalidOperationException("Restaurant test data has already been initialized.");
        }

        _dbContextOptions = new DbContextOptionsBuilder<RestaurantsDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        _currentUser = currentUser;
        _companyId = companyId;

        await using RestaurantsDbContext restaurantsDbContext = new(_dbContextOptions, currentUser);
        await restaurantsDbContext.Migrate(cancellationToken);

        Company company = Company.Create(
            new CompanyId(companyId),
            "My Home Ramen Integration",
            null,
            null,
            "My Home Ramen Integration Ltd",
            "PL123456789");
        Restaurant restaurant = Restaurant.Create(
            "Integration Restaurant",
            Address.Create("Main Street 1", "Warsaw", "00-001", Location.Create(52.2297, 21.0122)),
            true);
        restaurant.UpdateContactDetails("+48123456789", "integration@example.test");
        restaurant.UpdateBankAccount("1234567890", "Integration Bank", "123456789");
        company.AddRestaurant(restaurant);
        restaurantsDbContext.Companies.Add(company);
        await restaurantsDbContext.SaveChangesAsync(cancellationToken);

        RestaurantId = restaurant.Id.Value;
        return RestaurantId;
    }

    public async Task<Guid> SeedRestaurantAsync(string name, bool isActive, CancellationToken cancellationToken)
    {
        if (_dbContextOptions is null || _currentUser is null)
        {
            throw new InvalidOperationException("Restaurant test data has not been initialized.");
        }

        await using RestaurantsDbContext restaurantsDbContext = new(_dbContextOptions, _currentUser);
        Company company = await restaurantsDbContext.Companies.SingleAsync(
            candidate => candidate.Id == new CompanyId(_companyId),
            cancellationToken);
        Restaurant restaurant = Restaurant.Create(
            name,
            Address.Create($"{name} Street", "Warsaw", "00-001", Location.Create(52.2297, 21.0122)),
            isActive);

        company.AddRestaurant(restaurant);
        await restaurantsDbContext.SaveChangesAsync(cancellationToken);

        return restaurant.Id.Value;
    }
}
