using MyHomeRamen.Domain.Reservations.Permissions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Reservations.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class PermissionRepository(ReservationsDbContext reservationsDbContext, ICacheService cacheService)
    : BaseRepository<Permission, PermissionId>(reservationsDbContext, cacheService), IPermissionRepository
{
    public IPermissionQuery Query() => this;

    public IPermissionSpecification Specification() => this;
}