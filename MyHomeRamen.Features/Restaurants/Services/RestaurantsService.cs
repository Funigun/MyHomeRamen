using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Restaurants.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Services;

public sealed class RestaurantsService(IRestaurantDbContext dbContext) : IRestaurantsService
{
    public string? ValidateCompanyName(string companyName)
    {
        if (string.IsNullOrWhiteSpace(Company.NormalizeName(companyName)))
        {
            return "Company name is invalid.";
        }

        if (companyName.Length > CompanyConstants.MaxNameLength)
        {
            return $"Company name cannot exceed {CompanyConstants.MaxNameLength} characters.";
        }

        return null;
    }

    public Task<bool> IsNameUnique(string companyName, CancellationToken cancellationToken)
    {
        string normalizedName = Company.NormalizeName(companyName);
        return dbContext.Company.Query().IsNameUnique(normalizedName, cancellationToken);
    }

    public async Task<CompanyRegistrationResult> RegisterCompanyAsync(string companyName, CancellationToken cancellationToken)
    {
        Company company = Company.Create(companyName, null, null);
        company.UpdateBusinessDetails(companyName, "pending");
        dbContext.Company.Add(company);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompanyRegistrationResult(company.Id);
    }
}
