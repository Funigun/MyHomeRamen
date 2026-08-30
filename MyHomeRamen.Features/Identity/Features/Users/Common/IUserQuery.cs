using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.Common;

public interface IUserQuery
{
    Task<User?> ById(UserId userId, CancellationToken cancellationToken);

    Task<User?> ByGuestId(Guid guestId, CancellationToken cancellationToken);

    Task<Guid?> GetGuestIdByGuestIdAsync(Guid guestId, CancellationToken cancellationToken);

    Task<bool> AddressExists(Guid userId, Guid addressId, CancellationToken cancellationToken);

    Task<int> GetNumberOfAddresses(Guid userId, CancellationToken cancellationToken);

    Task<Guid?> GetIdByKeycloakId(string userId, CancellationToken cancellationToken);

    Task<User> SystemAccount(CancellationToken cancellationToken);
}
