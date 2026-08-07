using MyHomeRamen.Domain.Payments.Roles;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.Roles.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class RoleRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<Role, RoleId>(paymentsDbContext, cacheService), IRoleRepository
{
    public IRoleQuery Query() => this;

    public IRoleSpecification Specification() => this;
}