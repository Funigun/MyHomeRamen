using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.Identity.Features.CompanyMemberships.Common;

public interface ICompanyMembershipRepository : IRepository<CompanyMembership, CompanyMembershipId>
{
    Task<CompanyMembership?> ByIdempotencyKey(string key, CancellationToken cancellationToken);
    Task<bool> Exists(UserId userId, Guid companyId, CancellationToken cancellationToken);
    Task<bool> Exists(UserId userId, Guid companyId, Guid restaurantId, CancellationToken cancellationToken);
}


