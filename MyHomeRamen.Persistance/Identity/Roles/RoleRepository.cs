using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class RoleRepository(IdentityDbContext identityDbContext, ICacheService cacheService)
    : BaseRepository<Role, RoleId>(identityDbContext, cacheService), IRoleRepository
{
    public IRoleQuery Query() => this;

    public IRoleSpecification Specification() => this;
}