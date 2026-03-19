using System.Security.Claims;

using MyHomeRamen.Api.Common.Authorization;

namespace MyHomeRamen.IntegrationTests.Common.Configuration;

internal class FakeUser : ICurrentUser
{
    public string Id { get; init; } = "Fake_User";

    public Guid RestaurantId { get; init; } = Guid.Parse("fac13f05-5688-4169-9f89-927ae708dd35");

    public IEnumerable<Claim> Claims { get; init; } = [];
}
