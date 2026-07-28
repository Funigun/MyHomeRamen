using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Features.Menu.Features.Roles;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class RoleRepository(MenuDbContext menuDbContext) : BaseRepository<Role, RoleId>(menuDbContext), IRoleRepository
{
    IRoleQuery IRoleRepository.Query() => this;

    IRoleSpecification IRoleRepository.Specification() => this;
}
