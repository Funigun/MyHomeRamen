namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyQuery
{
    Task<bool> IsNameUnique(string name, CancellationToken cancellationToken);
}
