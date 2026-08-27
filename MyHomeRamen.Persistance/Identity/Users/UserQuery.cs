using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class UserRepository : IUserQuery
{
    public async Task<bool> AddressExists(Guid userId, Guid addressId, CancellationToken cancellationToken)
        => await identityDbContext.Users.AsNoTracking()
                      .Include(u => u.Addresses)
                      .Where(u => u.Id == new UserId(userId))
                      .AnyAsync(u => u.Addresses.Any(a => a.Id == addressId), cancellationToken);

    async Task<User?> IUserQuery.ById(UserId userId, CancellationToken cancellationToken)
        => await identityDbContext.Users.AsNoTracking()
                      .Include(user => user.Addresses)
                      .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public async Task<User?> ByGuestId(Guid guestId, CancellationToken cancellationToken)
    {
        CachePolicy cachePolicy = CachePolicy.LocalCache<IdentityCacheModule>("UserByGuestId_" + guestId, TimeSpan.FromMinutes(10), ["User"]);

        DbQueryOptions<User, User> options = new()
        {
            Filter = user => user.GuestId == guestId,
            Selector = user => user 
        };

        return await QueryFirstOrDefault(identityDbContext.Users, options, cachePolicy, cancellationToken);
    }

    public async Task<Guid?> GetGuestIdByGuestIdAsync(Guid guestId, CancellationToken cancellationToken)
        => await identityDbContext.Users.AsNoTracking()
                      .Where(u => u.GuestId == guestId)
                      .Select(u => u.GuestId)
                      .FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid?> GetIdByKeycloakId(string userId, CancellationToken cancellationToken)
        => await identityDbContext.Users.AsNoTracking()
                      .Where(u => u.KeycloakUserId == userId)
                      .Select(user => user.Id)
                      .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> GetNumberOfAddresses(Guid userId, CancellationToken cancellationToken)
    => await identityDbContext.Users.AsNoTracking()
                  .Include(u => u.Addresses)
                  .Where(u => u.Id == new UserId(userId))
                  .Select(u => u.Addresses.Count).FirstAsync(cancellationToken);

    public async Task<User> SystemAccount(CancellationToken cancellationToken)
    {
        string systemAccountName = "System";
        return await identityDbContext.Users.AsNoTracking()
                                            .FirstAsync(u => u.FirstName == systemAccountName, cancellationToken);
    }
}
