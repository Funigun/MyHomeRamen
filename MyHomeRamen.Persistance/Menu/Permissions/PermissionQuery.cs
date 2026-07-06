using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Permssions;
using MyHomeRamen.Features.Menu.Features.Permissions.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IPermissionQuery
{
    private IQueryable<Permission> PermissionsQuery => Permissions.AsNoTracking();
}
