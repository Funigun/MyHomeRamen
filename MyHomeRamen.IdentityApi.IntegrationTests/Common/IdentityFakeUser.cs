using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common;

internal sealed class IdentityFakeUser(DataSeeder dataSeeder) : ICurrentUser
{
    public string Id { get; init; } = dataSeeder.SeededUserKeycloakId;

    public Guid UserId { get; init; } = Guid.Empty;

    public Guid RestaurantId { get; init; } = dataSeeder.SeededRestaurantId;

    public IEnumerable<Claim> Claims { get; init; } = [];
}

internal static class IdentityFakeRestaurantConfig
{
    internal static RestaurantConfigurationProvider Create(DataSeeder dataSeeder)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["RestaurantConfiguration:RestaurantId"] = dataSeeder.SeededRestaurantId.ToString(),
                ["RestaurantConfiguration:Name"] = "TestRestaurant",
                ["RestaurantConfiguration:InfrastructurePrefix"] = "test"
            })
            .Build();

        return new RestaurantConfigurationProvider(config);
    }
}
