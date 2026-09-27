namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyQuery
{
    Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken);
}
