using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;
using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Persistance.Menu;

public partial class PermissionRepository(MenuDbContext menuDbContext, ICacheService cacheService) : BaseRepository<Permission, PermissionId>(menuDbContext, cacheService), IPermissionRepository
{
    IPermissionQuery IPermissionRepository.Query() => this;

    IPermissionSpecification IPermissionRepository.Specification() => this;
}
