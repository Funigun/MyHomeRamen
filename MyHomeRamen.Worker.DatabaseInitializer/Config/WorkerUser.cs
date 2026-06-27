using System.Security.Claims;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Worker.DatabaseInitializer.Config;

internal class WorkerUser(IConfiguration configuration) : ICurrentUser
{
    public string Id { get; init; } = "DB Migrator";

    public Guid UserId { get; init; } = Guid.Empty;

    public Guid RestaurantId { get; init; } = Guid.Parse(configuration["RestaurantConfiguration:RestaurantId"]!);

    public IEnumerable<Claim> Claims { get; init; } = [];
}
