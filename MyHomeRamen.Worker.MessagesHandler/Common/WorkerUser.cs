using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Worker.MessagesHandler.Common;

internal class WorkerUser(IConfiguration configuration) : ICurrentUser
{
    public string Id { get; init; } = "Messages Worker";

    public Guid UserId { get; init; } = Guid.Empty;

    public Guid RestaurantId { get; init; } = Guid.Parse(configuration["RestaurantConfiguration:RestaurantId"]!);

    public IEnumerable<Claim> Claims { get; init; } = [];
}
