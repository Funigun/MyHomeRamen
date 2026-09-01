using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.IntegrationTests.Authentication;

public class FakeUser : ICurrentUser
{
    public string IdentityId { get; init; } = "Fake_User";

    public Guid UserId { get; init; } = Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = [];
    public bool IsAuthenticated { get; init; }
    public bool IsGuest { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
