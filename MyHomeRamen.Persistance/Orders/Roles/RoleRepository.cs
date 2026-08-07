using MyHomeRamen.Domain.Orders.Roles;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Roles.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class RoleRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<Role, RoleId>(ordersDbContext, cacheService), IRoleRepository
{
    public IRoleQuery Query() => this;

    public IRoleSpecification Specification() => this;
}