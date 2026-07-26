using DotNet.Testcontainers.Builders;
using MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;
using Testcontainers.MsSql;

[assembly: AssemblyFixture(typeof(DbContainerFixture))]

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

public sealed class DbContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                                                  .WithPortBinding(14331, 1433)
                                                                  .WithPassword("Str0ng_P@ssw0rd4Tests")
                                                                  .WithEnvironment("ACCEPT_EULA", "Y")
                                                                  .WithName("MyHomeRamenIdentityTestDb")
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
