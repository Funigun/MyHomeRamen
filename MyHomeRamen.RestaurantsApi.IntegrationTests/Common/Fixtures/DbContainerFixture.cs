using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(MyHomeRamen.RestaurantsApi.IntegrationTests.Common.Fixtures.DbContainerFixture))]

namespace MyHomeRamen.RestaurantsApi.IntegrationTests.Common.Fixtures;

public sealed class DbContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
        .WithPassword("Str0ng_P@ssw0rd4Tests")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithCleanUp(true)
        .Build();

    internal string ConnectionString => _sqlContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _sqlContainer.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }
}
