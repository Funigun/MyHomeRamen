using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class UserRepository : IUserSpecification
{
    public async Task<User?> ByExternalId(string seededUserKeycloakId, CancellationToken cancellationToken)
        => await identityDbContext.Users.Include(u => u.Addresses).FirstOrDefaultAsync(user => user.KeycloakUserId == seededUserKeycloakId, cancellationToken);

    public async Task<User> ById(UserId userId, CancellationToken cancellationToken)
        => await identityDbContext.Users.Include(u => u.Addresses).FirstAsync(user => user.Id == userId, cancellationToken);
}
