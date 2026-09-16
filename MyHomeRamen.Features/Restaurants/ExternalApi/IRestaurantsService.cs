namespace MyHomeRamen.Features.Restaurants.ExternalApi;

public sealed record CompanyRegistrationResult(Guid CompanyId);

public interface IRestaurantsService
{
    string? ValidateCompanyName(string companyName);

    Task<bool> IsNameUnique(string companyName, CancellationToken cancellationToken);

    Task<CompanyRegistrationResult> RegisterCompanyAsync(string companyName, CancellationToken cancellationToken);
}
