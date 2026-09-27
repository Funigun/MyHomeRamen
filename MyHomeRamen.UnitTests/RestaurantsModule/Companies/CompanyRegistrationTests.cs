using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.UnitTests.RestaurantsModule.Companies;

public sealed class CompanyRegistrationTests
{
    [Fact]
    public void Create_ShouldPreserveId_WhenIdIsProvided()
    {
        CompanyId companyId = new(Guid.CreateVersion7());

        Company company = Company.Create(companyId, "My Home Ramen", null, null);

        Assert.Equal(companyId, company.Id);
    }

    [Fact]
    public void Create_ShouldNormalizeName_WhenIdIsProvided()
    {
        Company company = Company.Create(new CompanyId(Guid.CreateVersion7()), "My Home Ramen", null, null);

        Assert.Equal("MYHOMERAMEN", company.NormalizedName);
    }
}
