using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class PermissionRepository(MenuDbContext menuDbContext) : BaseRepository<Permission, PermissionId>(menuDbContext), IPermissionRepository
{
    IPermissionQuery IPermissionRepository.Query() => this;

    IPermissionSpecification IPermissionRepository.Specification() => this;
}
