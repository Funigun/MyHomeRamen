using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.Common;

public interface IUserLoader
{
    Task<User?> ByExternalId(string seededUserKeycloakId, CancellationToken cancellationToken);
    Task<User> ById(UserId userId, CancellationToken cancellationToken);
}
