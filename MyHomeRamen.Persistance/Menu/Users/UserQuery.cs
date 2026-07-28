using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class UserRepository : IUserQuery
{
    private IQueryable<User> UsersQuery => menuDbContext.Users.AsNoTracking();
}
