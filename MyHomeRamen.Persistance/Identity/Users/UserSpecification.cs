using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Persistance.Identity;

public partial class IdentityDbContext : IUserSpecification
{
    public async Task<User?> ByExternalId(string seededUserKeycloakId, CancellationToken cancellationToken)
        => await Users.FirstOrDefaultAsync(user => user.KeycloakUserId == seededUserKeycloakId, cancellationToken);

    public async Task<User> ById(UserId userId, CancellationToken cancellationToken)
        => await Users.FirstAsync(user => user.Id == userId, cancellationToken);
}
