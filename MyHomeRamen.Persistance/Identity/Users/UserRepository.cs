using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Identity;

public sealed partial class UserRepository(IdentityDbContext identityDbContext, ICacheService cacheService) : BaseRepository<User, UserId>(identityDbContext, cacheService), IUserRepository
{
    public IUserQuery Query() => this;

    public IUserSpecification Specification() => this;
}
