using MyHomeRamen.Features.Restaurants.Features.Companies.Common;

namespace MyHomeRamen.Persistance.Restaurants;

public partial class CompanyRepository : ICompanyQuery
{
    public async Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken)
        => !await Exists(c => c.NormalizedName == normalizedName, cancellationToken);
}
