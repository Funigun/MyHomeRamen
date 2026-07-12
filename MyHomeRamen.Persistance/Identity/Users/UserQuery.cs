using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class IdentityDbContext : IUserQuery
{
    public async Task<bool> AddressExists(Guid userId, Guid addressId, CancellationToken cancellationToken)
        => await Users.AsNoTracking()
                      .Include(u => u.Addresses)
                      .Where(u => u.Id == new UserId(userId))
                      .AnyAsync(u => u.Addresses.Any(a => a.Id == addressId), cancellationToken);

    async Task<User?> IUserQuery.ById(UserId userId, CancellationToken cancellationToken)
        => await Users.AsNoTracking()
                      .Include(user => user.Addresses)
                      .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public async Task<Guid?> GetGuestIdByGuestIdAsync(Guid guestId, CancellationToken cancellationToken)
        => await Users.AsNoTracking()
                      .Where(u => u.GuestId == guestId)
                      .Select(u => u.GuestId)
                      .FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid?> GetIdByKeycloakId(string userId, CancellationToken cancellationToken)
        => await Users.AsNoTracking()
                      .Where(u => u.KeycloakUserId == userId)
                      .Select(user => user.Id)
                      .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> GetNumberOfAddresses(Guid userId, CancellationToken cancellationToken)
    => await Users.AsNoTracking()
                  .Include(u => u.Addresses)
                  .Where(u => u.Id == new UserId(userId))
                  .Select(u => u.Addresses.Count).FirstAsync(cancellationToken);
}
