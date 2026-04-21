using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal sealed class IdentityFakeUser : ICurrentUser
{
    public string Id { get; init; } = DataSeeder.SeededUserKeycloakId;

    public Guid RestaurantId { get; init; } = DataSeeder.SeededRestaurantId;

    public IEnumerable<Claim> Claims { get; init; } = [];
}

internal static class IdentityFakeRestaurantConfig
{
    internal static RestaurantConfigurationProvider Create()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RestaurantConfiguration:RestaurantId"] = DataSeeder.SeededRestaurantId.ToString(),
                ["RestaurantConfiguration:Name"] = "TestRestaurant",
                ["RestaurantConfiguration:InfrastructurePrefix"] = "test"
            })
            .Build();

        return new RestaurantConfigurationProvider(config);
    }
}
