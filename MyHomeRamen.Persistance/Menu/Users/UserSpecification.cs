using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Users.Common;

namespace MyHomeRamen.Persistance.Menu;

public partial class MenuDbContext : IUserSpecification
{
    public async Task<User> ById(UserId userId, CancellationToken cancellationToken)
        => await Users.Include(user => user.FavoriteProducts)
                      .Include(user => user.Roles)
                      .Include(user => user.Permissions)
                      .AsSplitQuery()
                      .FirstAsync(user => user.Id == userId, cancellationToken);
}
