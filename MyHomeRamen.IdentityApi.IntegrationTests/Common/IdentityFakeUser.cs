using System.Security.Claims;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal sealed class IdentityFakeUser(DataSeeder dataSeeder) : ICurrentUser
{
    public string IdentityId { get; init; } = dataSeeder.SeededUserKeycloakId;

    public Guid UserId { get; init; } = Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = [];
    public bool IsAuthenticated { get; init; }
    public bool IsGuest { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
