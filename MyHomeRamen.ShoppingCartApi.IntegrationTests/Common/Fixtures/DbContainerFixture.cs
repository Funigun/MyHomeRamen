using DotNet.Testcontainers.Builders;
using MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DbContainerFixture))]

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Fixtures;

public sealed class DbContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
        .WithPassword("Str0ng_P@ssw0rd4Tests")
        .WithPortBinding(1400)
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithName("MyHomeRamenShoppingCartTestDb")
        .WithCleanUp(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1410))
        .Build();

    internal string ConnectionString => _sqlContainer.GetConnectionString();

    public async ValueTask InitializeAsync() => await _sqlContainer.StartAsync();

    public async ValueTask DisposeAsync() => await _sqlContainer.DisposeAsync();
}
