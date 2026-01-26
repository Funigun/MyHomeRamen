using System.Reflection;

namespace MyHomeRamen.ArchitectureTests.Common;

public abstract class BaseArchitectureTest
{
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
    protected static readonly Assembly ArchitectureTestsAssembly = typeof(BaseArchitectureTest).Assembly;

    protected static readonly Assembly[] ProjectAssemblies =
    [
        ApiAssembly,
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
        ArchitectureTestsAssembly
    ];
}
