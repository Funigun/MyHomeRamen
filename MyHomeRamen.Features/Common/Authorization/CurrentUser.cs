using System.Security.Claims;

namespace MyHomeRamen.Features.Common.Authorization;

public sealed record CurrentUser : ICurrentUser
{
    public required string IdentityId { get; init; }

    public required Guid UserId { get; init; }

    public IEnumerable<Claim> Claims { get; init; } = [];

    public required bool IsAuthenticated { get; init; }

    public required bool IsGuest { get; init; }

    public IReadOnlyCollection<string> Permissions { get; init; } = [];

    public static CurrentUser Anonymous { get; } = new()
    {
        IdentityId = string.Empty,
        UserId = Guid.Empty,
        IsAuthenticated = false,
        IsGuest = true,
    };
}
