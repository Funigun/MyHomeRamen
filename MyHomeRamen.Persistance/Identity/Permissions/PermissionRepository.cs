using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class PermissionRepository(IdentityDbContext identityDbContext, ICacheService cacheService) 
                          : BaseRepository<Permission, PermissionId>(identityDbContext, cacheService), IPermissionRepository
{
    public IPermissionQuery Query() => this;

    public IPermissionLoader Load() => this;
}
