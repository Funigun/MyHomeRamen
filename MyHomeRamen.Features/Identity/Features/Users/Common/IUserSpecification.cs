using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.Features.Identity.Features.Users.Common;

public interface IUserSpecification
{
    Task<User?> ByExternalId(string seededUserKeycloakId, CancellationToken cancellationToken);
    Task<User> ById(UserId userId, CancellationToken cancellationToken);
}
