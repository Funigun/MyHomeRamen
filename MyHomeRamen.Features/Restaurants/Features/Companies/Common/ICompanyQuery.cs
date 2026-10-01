using MyHomeRamen.Domain.Restaurants.Companies;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.Common;

public interface ICompanyQuery
{
    Task<bool> HasMedia(SocialMediaId mediaId, CancellationToken cancellationToken);

    Task<bool> IsNameUnique(string normalizedName, CancellationToken cancellationToken);
}
