using DotNet.Testcontainers.Builders;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;
using Testcontainers.Redis;

[assembly: AssemblyFixture(typeof(RedisFixture))]

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;

public sealed class RedisFixture : IAsyncLifetime
{
    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:8.2").Build();

    internal string ConnectionString => _redisContainer.GetConnectionString();

    public async ValueTask InitializeAsync() => await _redisContainer.StartAsync();

    public async ValueTask DisposeAsync() => await _redisContainer.DisposeAsync();
}
