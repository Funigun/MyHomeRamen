using MyHomeRamen.Domain.Reservations.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Reservations.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Reservations;

public sealed partial class UserRepository(ReservationsDbContext reservationsDbContext, ICacheService cacheService)
    : BaseRepository<User, UserId>(reservationsDbContext, cacheService), IUserRepository
{
    public IUserQuery Query() => this;

    public IUserSpecification Specification() => this;
}