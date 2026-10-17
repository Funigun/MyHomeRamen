using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Identity.Features.CompanyMemberships.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Identity.CompanyMemberships;

public sealed class CompanyMembershipRepository(IdentityDbContext dbContext, ICacheService cacheService) : BaseRepository<CompanyMembership, CompanyMembershipId>(dbContext, cacheService), ICompanyMembershipRepository
{
    public async Task<bool> Exists(UserId userId, Guid companyId, CancellationToken cancellationToken)
        => await dbContext.CompanyMemberships.AsNoTracking().AnyAsync(x => x.UserId == userId && x.CompanyId == companyId, cancellationToken);

    public async Task<bool> Exists(UserId userId, Guid companyId, Guid restaurantId, CancellationToken cancellationToken)
        => await dbContext.CompanyMemberships.AsNoTracking().AnyAsync(x => x.UserId == userId && x.CompanyId == companyId && (x.RestaurantId == null || x.RestaurantId == restaurantId), cancellationToken);
}
