using DotNet.Testcontainers.Builders;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;
using Testcontainers.Redis;

[assembly: AssemblyFixture(typeof(RedisFixture))]

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Fixtures;

public sealed class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:8.2")
                                                            .WithPortBinding(1250)
                                                            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1260))
                                                            .Build();

    internal string ConnectionString => _redisContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _redisContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _redisContainer.DisposeAsync();
    }
}
