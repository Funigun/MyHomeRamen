using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public interface ICurrentUser
{
    string Id { get; }

    Guid UserId { get; }

    IEnumerable<Claim> Claims { get; init; }
}
