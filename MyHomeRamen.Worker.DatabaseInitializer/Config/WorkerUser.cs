using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Worker.DatabaseInitializer.Config;

internal sealed class WorkerUser : ICurrentUser
{
    public string Id { get; init; } = "DB Migrator";

    public Guid UserId { get; init; } = Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = [];
    public bool IsAuthenticated { get; init; }
    public bool IsGuest { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
