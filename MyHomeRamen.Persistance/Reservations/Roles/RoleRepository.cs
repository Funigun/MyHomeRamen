using MyHomeRamen.Domain.Reservations.Roles;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Reservations.Features.Roles.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class RoleRepository(ReservationsDbContext reservationsDbContext, ICacheService cacheService)
    : BaseRepository<Role, RoleId>(reservationsDbContext, cacheService), IRoleRepository
{
    public IRoleQuery Query() => this;

    public IRoleSpecification Specification() => this;
}