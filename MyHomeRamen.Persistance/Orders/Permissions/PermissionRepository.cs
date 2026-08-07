using MyHomeRamen.Domain.Orders.Permissions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class PermissionRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Permission, PermissionId>(ordersDbContext, cacheService), IPermissionRepository
{
    public IPermissionQuery Query() => this;

    public IPermissionSpecification Specification() => this;
}