using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Persistance.Users.Extensions;

public static partial class DbExtensions
{
    extension(IQueryable<User> users)
    {
        public async Task<User?> GetByKeycloakIdQuery(string userId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .FirstOrDefaultAsync(u => u.KeycloakUserId == userId, cancellationToken);
        }

        public async Task<Guid?> GetIdByKeycloakId(string userId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .Where(u => u.KeycloakUserId == userId)
                              .Select(user => user.Id)
                              .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetById(Guid userId, CancellationToken cancellationToken)
        {
            return await users.Include(u => u.Addresses)
                              .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<User?> GetByIdQuery(Guid userId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .Include(u => u.Addresses)
                              .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<int> GetNumberOfAddresses(Guid userId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .Include(u => u.Addresses)
                              .Where(u => u.Id == userId)
                              .Select(u => u.Addresses.Count).FirstAsync(cancellationToken);
        }

        public async Task<bool> AddressExists(Guid userId, Guid addressId, CancellationToken cancellationToken)
        {
            return await users.AsNoTracking()
                              .Include(u => u.Addresses)
                              .Where(u => u.Id == userId)
                              .AnyAsync(u => u.Addresses.Any(a => a.Id == addressId), cancellationToken);
        }
    }
}
