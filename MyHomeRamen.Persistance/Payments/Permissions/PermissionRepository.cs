using MyHomeRamen.Domain.Payments.Permissions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class PermissionRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<Permission, PermissionId>(paymentsDbContext, cacheService), IPermissionRepository
{
    public IPermissionQuery Query() => this;

    public IPermissionSpecification Specification() => this;
}