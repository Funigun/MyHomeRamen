using ArchUnitNET.Domain;
using Assembly = System.Reflection.Assembly;

using static ArchUnitNET.Fluent.ArchRuleDefinition; 

namespace MyHomeRamen.ArchitectureTests.Common;

public abstract class BaseArchitectureTest(ArchitectureBuilder architectureBuilder) : IAsyncLifetime
{
    protected static readonly Assembly ApiContractsAssembly = typeof(MyHomeRamen.Common.Contracts.ICommonContractsAssemblyMarker).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(MyHomeRamen.Api.IApiAssemblyMarker).Assembly;
    protected static readonly Assembly ApiCommonAssembly = typeof(MyHomeRamen.Api.Common.DependencyInjection).Assembly;
    protected static readonly Assembly AppHostAssembly = typeof(MyHomeRamen.AppHost.IAppHostAssemblyMarker).Assembly;
    protected static readonly Assembly BlazorClientAssembly = typeof(MyHomeRamen.Blazor.Client.IBlazorClientAssemblyMarker).Assembly;
    protected static readonly Assembly BlazorServerAssembly = typeof(MyHomeRamen.Blazor.Components.App).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(MyHomeRamen.Domain.IDomainAssemblyMarker).Assembly;
    protected static readonly Assembly IdentityApiAssembly = typeof(MyHomeRamen.Identity.Api.IIdentityApiAssemblyMarker).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(MyHomeRamen.Infrastructure.IInfrastructureAssemblyMarker).Assembly;
    protected static readonly Assembly IntegrationTestsAssembly = typeof(MyHomeRamen.IntegrationTests.IIntegrationTestsAssemblyMarker).Assembly;
    protected static readonly Assembly PersistanceAssembly = typeof(MyHomeRamen.Persistance.IPersistanceAssemblyMarker).Assembly;
    protected static readonly Assembly ServiceDefaultsAssembly = typeof(MyHomeRamen.ServiceDefaults.IServiceDefaultsAssemblyMarker).Assembly;
    protected static readonly Assembly UnitTestsAssembly = typeof(MyHomeRamen.UnitTests.IUnitTestsAssemblyMarker).Assembly;
    protected static readonly Assembly WorkerCommonAssembly = typeof(MyHomeRamen.Worker.Common.IWorkerCommonAssemblyMarker).Assembly;
    protected static readonly Assembly WorkerMailSenderAssembly = typeof(MyHomeRamen.Worker.MailSender.IWorkerMailSenderAssemblyMarker).Assembly;
    protected static readonly Assembly WorkerMessagesHandlerAssembly = typeof(MyHomeRamen.Worker.MessagesHandler.IWorkerMessagesHandlerAssemblyMarker).Assembly;
    protected static readonly Assembly WorkerDbInitializerAssembly = typeof(MyHomeRamen.Worker.DatabaseInitializer.IDbInitializerWorkerAssemblyMarker).Assembly;
    protected static readonly Assembly ArchitectureTestsAssembly = typeof(BaseArchitectureTest).Assembly;

    protected IObjectProvider<IType> ApiLayer { get; private set; } = Types().That().ResideInAssembly(ApiAssembly).As("API Layer");

    protected IObjectProvider<IType> ApiContractsLayer { get; private set; } = Types().That().ResideInAssembly(ApiContractsAssembly).As("API Contracts Layer");

    protected IObjectProvider<IType> ApiCommonLayer { get; private set; } = Types().That().ResideInAssembly(ApiCommonAssembly).As("API Common Layer");

    protected IObjectProvider<IType> AppHostLayer { get; private set; } = Types().That().ResideInAssembly(AppHostAssembly).As("App Host Layer");

    protected IObjectProvider<IType> BlazorClientLayer { get; private set; } = Types().That().ResideInAssembly(BlazorClientAssembly).As("Blazor Client Layer");

    protected IObjectProvider<IType> BlazorServerLayer { get; private set; } = Types().That().ResideInAssembly(BlazorServerAssembly).As("Blazor Server Layer");

    protected IObjectProvider<IType> DomainLayer { get; private set; } = Types().That().ResideInAssembly(DomainAssembly).As("Domain Layer");

    protected IObjectProvider<IType> IdentityApiLayer { get; private set; } = Types().That().ResideInAssembly(IdentityApiAssembly).As("Identity API Layer");

    protected IObjectProvider<IType> InfrastructureLayer { get; private set; } = Types().That().ResideInAssembly(InfrastructureAssembly).As("Infrastructure Layer");

    protected IObjectProvider<IType> IntegrationTestsLayer { get; private set; } = Types().That().ResideInAssembly(IntegrationTestsAssembly).As("Integration Tests Layer");

    protected IObjectProvider<IType> PersistanceLayer { get; private set; } = Types().That().ResideInAssembly(PersistanceAssembly).As("Persistance Layer");

    protected IObjectProvider<IType> ServiceDefaultsLayer { get; private set; } = Types().That().ResideInAssembly(ServiceDefaultsAssembly).As("Service Defaults Layer");

    protected IObjectProvider<IType> UnitTestsLayer { get; private set; } = Types().That().ResideInAssembly(UnitTestsAssembly).As("Unit Tests Layer");

    protected IObjectProvider<IType> WorkerCommonLayer { get; private set; } = Types().That().ResideInAssembly(WorkerCommonAssembly).As("Worker Common Layer");

    protected IObjectProvider<IType> WorkerMailSenderLayer { get; private set; } = Types().That().ResideInAssembly(WorkerMailSenderAssembly).As("Worker Mail Sender Layer");

    protected IObjectProvider<IType> WorkerMessagesHandlerLayer { get; private set; } = Types().That().ResideInAssembly(WorkerMessagesHandlerAssembly).As("Worker Messages Handler Layer");

    protected IObjectProvider<IType> WorkerDbInitializerLayer { get; private set; } = Types().That().ResideInAssembly(WorkerDbInitializerAssembly).As("Worker DB Initializer Layer");

    protected IObjectProvider<IType> ArchitectureTestsLayer { get; private set; } = Types().That().ResideInAssembly(ArchitectureTestsAssembly).As("Architecture Tests Layer");

    protected static readonly Assembly[] ProjectAssemblies =
    [
        ApiAssembly,
        ApiContractsAssembly,
        ApiCommonAssembly,
        AppHostAssembly,
        BlazorClientAssembly,
        BlazorServerAssembly,
        DomainAssembly,
        IdentityApiAssembly,
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

    protected Architecture Architecture => architectureBuilder.Architecture;

    public async ValueTask InitializeAsync()
    {
        architectureBuilder.Setup(ProjectAssemblies);
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {

    }
}
