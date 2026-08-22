using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public interface ICurrentUser
{
    string Id { get; init; }

    Guid UserId { get; init; }

    IEnumerable<Claim> Claims { get; init; }

    bool IsAuthenticated { get; init; }

    bool IsGuest { get; init; }

    IReadOnlyCollection<string> Permissions { get; init; }
}
