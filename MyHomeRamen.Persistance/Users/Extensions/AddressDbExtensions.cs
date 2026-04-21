using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Persistance.Users.Extensions;

public static partial class DbExtensions
{
    extension(IQueryable<User> users)
    {
        public async Task<int> GetNumberOfAddresses(Guid userId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .Include(u => u.Addresses)
                              .Where(u => u.Id == userId)
                              .Select(u => u.Addresses.Count).FirstAsync(cancellationToken);
        }
    }
}
