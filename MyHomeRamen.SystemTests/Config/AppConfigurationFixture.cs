using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using MyHomeRamen.SystemTests.Config;
using static System.Net.Mime.MediaTypeNames;

[assembly: AssemblyFixture(typeof(AppConfigurationFixture))]

namespace MyHomeRamen.SystemTests.Config;

public sealed class AppConfigurationFixture : IAsyncLifetime
{
    public const string InfrastructurePrefix = "my-home-ramen";
    public const string IdentityApiResourceName = $"{InfrastructurePrefix}-identity-api";
    public const string ConnectionString = "<TO_BE_UPDATED>";

    private static readonly string[] _configuration =
    [
        "DatabaseConfiguration:DatabaseName=MyHomeRamenTest",
        $"RestaurantConfiguration:InfrastructurePrefix={InfrastructurePrefix}",
    ];

    public IDistributedApplicationTestingBuilder ApplicationBuilder { get; private set; } = default!;

    public DistributedApplication Application { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        ApplicationBuilder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyHomeRamen_Api>(_configuration, cancellationToken);

        Application = await ApplicationBuilder.BuildAsync(CancellationToken.None)
                                              .WaitAsync(TimeSpan.FromSeconds(60), cancellationToken);

        await Application.StartAsync(cancellationToken)
                          .WaitAsync(TimeSpan.FromSeconds(300), cancellationToken);

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-cache", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-rabbitmq", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-key-cloak", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-db-initializer", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(60));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-messages-worker", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-identity-api", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));

        await Application.ResourceNotifications.WaitForResourceAsync($"{InfrastructurePrefix}-api", KnownResourceStates.Running, cancellationToken)
                                               .WaitAsync(TimeSpan.FromSeconds(30));
    }

    public async ValueTask DisposeAsync()
    {
        await Application.DisposeAsync();
        await ApplicationBuilder.DisposeAsync();
    }
}
