using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Worker.MessagesHandler.Common;

internal sealed class WorkerUser : ICurrentUser
{
    public string Id { get; init; } = "Messages Worker";

    public Guid UserId { get; init; } = Guid.Empty;

    public IEnumerable<Claim> Claims { get; init; } = [];
    public bool IsAuthenticated { get; init; }
    public bool IsGuest { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
