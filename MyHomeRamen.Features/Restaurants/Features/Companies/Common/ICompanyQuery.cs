using MyHomeRamen.Features.Restaurants.Features.Companies.GetDetails;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyQuery
{
    Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken);

    Task<CompanyDetailsDto?> CurrentDetails(CancellationToken cancellationToken);
}
