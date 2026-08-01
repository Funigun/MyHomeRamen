using MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Configuration;

public sealed class IdentityApiFixture(DbContainerFixture dbContainerFixture) : IAsyncLifetime
{
    private readonly DataSeeder _dataSeeder = new();

    public IdentityWebApiFactory ApiFactory { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        ApiFactory = new IdentityWebApiFactory(dbContainerFixture, _dataSeeder);
        await ((IAsyncLifetime)ApiFactory).InitializeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (ApiFactory is not null)
        {
            await ApiFactory.DisposeAsync();
        }
    }
}
