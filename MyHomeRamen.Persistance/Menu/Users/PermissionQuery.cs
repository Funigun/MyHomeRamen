using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IPermissionQuery
{
    private IQueryable<Permission> PermissionsQuery => Permissions.AsNoTracking();
}
