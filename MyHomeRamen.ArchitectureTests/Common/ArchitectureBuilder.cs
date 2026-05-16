using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using MyHomeRamen.ArchitectureTests.Common;

[assembly: AssemblyFixture(typeof(ArchitectureBuilder))]

namespace MyHomeRamen.ArchitectureTests.Common;

public sealed class ArchitectureBuilder : IAsyncLifetime
{
    public Architecture Architecture { get; private set; } = default!;

    public System.Reflection.Assembly ApiContractsAssembly { get; private set; } = typeof(MyHomeRamen.Common.Contracts.ICommonContractsAssemblyMarker).Assembly;

    public System.Reflection.Assembly ApiAssembly { get; private set; } = typeof(Api.IApiAssemblyMarker).Assembly;

    public System.Reflection.Assembly ApiCommonAssembly { get; private set; } = typeof(Api.Common.DependencyInjection).Assembly;

    public System.Reflection.Assembly AppHostAssembly { get; private set; } = typeof(AppHost.IAppHostAssemblyMarker).Assembly;

    public System.Reflection.Assembly BlazorClientAssembly { get; private set; } = typeof(Blazor.Client.IBlazorClientAssemblyMarker).Assembly;

    public System.Reflection.Assembly BlazorServerAssembly { get; private set; } = typeof(Blazor.Components.App).Assembly;

    public System.Reflection.Assembly DomainAssembly { get; private set; } = typeof(Domain.IDomainAssemblyMarker).Assembly;

    public System.Reflection.Assembly InfrastructureAssembly { get; private set; } = typeof(Infrastructure.IInfrastructureAssemblyMarker).Assembly;

    public System.Reflection.Assembly IntegrationTestsAssembly { get; private set; } = typeof(IntegrationTests.IIntegrationTestsAssemblyMarker).Assembly;

    public System.Reflection.Assembly PersistanceAssembly { get; private set; } = typeof(Persistance.IPersistanceAssemblyMarker).Assembly;

    public System.Reflection.Assembly ServiceDefaultsAssembly { get; private set; } = typeof(ServiceDefaults.IServiceDefaultsAssemblyMarker).Assembly;

    public System.Reflection.Assembly UnitTestsAssembly { get; private set; } = typeof(UnitTests.IUnitTestsAssemblyMarker).Assembly;

    public System.Reflection.Assembly WorkerCommonAssembly { get; private set; } = typeof(Worker.Common.IWorkerCommonAssemblyMarker).Assembly;

    public System.Reflection.Assembly WorkerMailSenderAssembly { get; private set; } = typeof(Worker.MailSender.IWorkerMailSenderAssemblyMarker).Assembly;

    public System.Reflection.Assembly WorkerMessagesHandlerAssembly { get; private set; } = typeof(Worker.MessagesHandler.IWorkerMessagesHandlerAssemblyMarker).Assembly;

    public System.Reflection.Assembly WorkerDbInitializerAssembly { get; private set; } = typeof(Worker.DatabaseInitializer.IDbInitializerWorkerAssemblyMarker).Assembly;

    public System.Reflection.Assembly ArchitectureTestsAssembly { get; private set; } = typeof(BaseArchitectureTest).Assembly;

    public System.Reflection.Assembly[] AllAssemblies { get; private set; }

    public void Setup(IEnumerable<System.Reflection.Assembly> assemblies)
    {
        if (Architecture == default)
        {
            Architecture = new ArchLoader().LoadAssemblies(assemblies.ToArray()).Build();
        }
    }

    public async ValueTask InitializeAsync()
    {
        AllAssemblies =
        [
            ApiAssembly,
            ApiContractsAssembly,
            ApiCommonAssembly,
            AppHostAssembly,
            BlazorClientAssembly,
            BlazorServerAssembly,
            DomainAssembly,
            InfrastructureAssembly,
            IntegrationTestsAssembly,
            PersistanceAssembly,
            ServiceDefaultsAssembly,
            UnitTestsAssembly,
            WorkerCommonAssembly,
            WorkerMailSenderAssembly,
            WorkerMessagesHandlerAssembly,
            WorkerDbInitializerAssembly,
            ArchitectureTestsAssembly
        ];

        Architecture = new ArchLoader().LoadAssemblies(AllAssemblies.ToArray()).Build();
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
    }
}
