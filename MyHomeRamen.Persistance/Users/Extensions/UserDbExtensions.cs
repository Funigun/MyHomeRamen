using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    public static async Task<Guid?> GetGuestIdByGuestIdAsync(this IQueryable<User> users, Guid guestId, CancellationToken cancellationToken)
        => await users.AsNoTracking()
                      .Where(u => u.GuestId == guestId)
                      .Select(u => u.GuestId)
                      .FirstOrDefaultAsync(cancellationToken);
}
