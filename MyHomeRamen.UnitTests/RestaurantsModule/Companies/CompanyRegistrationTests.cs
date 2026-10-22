using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Domain.Restaurants.Restaurants;
using MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Companies;

public sealed class CompanyRegistrationTests
{
    [Fact]
    public void Create_ShouldPreserveId_WhenIdIsProvided()
    {
        CompanyId companyId = new(Guid.CreateVersion7());

        Company company = Company.Create(companyId, "My Home Ramen", null, null, "My Home Ramen Ltd", "PL123456789");

        Assert.Equal(companyId, company.Id);
    }

    [Fact]
    public void Create_ShouldNormalizeName_WhenIdIsProvided()
    {
        Company company = Company.Create(new CompanyId(Guid.CreateVersion7()), "My Home Ramen", null, null, "My Home Ramen Ltd", "PL123456789");

        Assert.Equal("MYHOMERAMEN", company.NormalizedName);
    }

    [Fact]
    public void AddRestaurant_ShouldAssociateRestaurantWithCompany()
    {
        Company company = Company.Create("My Home Ramen", null, null, "My Home Ramen Ltd", "PL123456789");
        Restaurant restaurant = Restaurant.Create(
            "Downtown",
            Address.Create("Main Street 1", "Warsaw", "00-001", Location.Create(52.2297, 21.0122)),
            true);

        company.AddRestaurant(restaurant);

        Assert.Contains(restaurant, company.Restaurants);
    }
}
