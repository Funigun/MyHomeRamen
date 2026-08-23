using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public interface ICurrentUser
{
    string IdentityId { get; }

    Guid UserId { get; }

    IEnumerable<Claim> Claims { get; }

    bool IsAuthenticated { get; }

    bool IsGuest { get; }

    IReadOnlyCollection<string> Permissions { get; }
}
