using System.Security.Claims;
using MyHomeRamen.Api.Common.Authorization;

namespace MyHomeRamen.Worker.DatabaseInitializer.Config;

internal class Worker : ICurrentUser
{
    public string Id { get; init; } = "DB Migrator";

    public IEnumerable<Claim> Claims { get; init; } = [];
}
