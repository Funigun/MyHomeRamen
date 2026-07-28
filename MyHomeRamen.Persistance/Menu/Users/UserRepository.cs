using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Menu;

public sealed partial class UserRepository(MenuDbContext menuDbContext) : BaseRepository<User, UserId>(menuDbContext), IUserRepository
{
    IUserQuery IUserRepository.Query() => this;

    IUserSpecification IUserRepository.Specification() => this;
}
