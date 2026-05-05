using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension(IQueryable<User> users)
    {
        public async Task<User?> FindByIdAsync(UserId userId, CancellationToken cancellationToken = default)
            => await users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
