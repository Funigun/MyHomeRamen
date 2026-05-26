using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.Common;

public abstract class BaseArchitectureTest(ArchitectureBuilder architectureBuilder)
{
    protected ArchitectureBuilder ArchitectureBuilder { get; } = architectureBuilder;

    protected IObjectProvider<IType> ApiLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.ApiAssembly).As("API Layer");

    protected IObjectProvider<IType> ApiContractsLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.ApiContractsAssembly).As("API Contracts Layer");

    protected IObjectProvider<IType> ApiCommonLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.ApiCommonAssembly).As("API Common Layer");

    protected IObjectProvider<IType> AppHostLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.AppHostAssembly).As("App Host Layer");

    protected IObjectProvider<IType> BlazorClientLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.BlazorClientAssembly).As("Blazor Client Layer");

    protected IObjectProvider<IType> BlazorServerLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.BlazorServerAssembly).As("Blazor Server Layer");

    protected IObjectProvider<IType> DomainLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.DomainAssembly).As("Domain Layer");

    protected IObjectProvider<IType> InfrastructureLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.InfrastructureAssembly).As("Infrastructure Layer");

    protected IObjectProvider<IType> IntegrationTestsLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.IntegrationTestsAssembly).As("Integration Tests Layer");

    protected IObjectProvider<IType> PersistanceLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.PersistanceAssembly).As("Persistance Layer");

    protected IObjectProvider<IType> ServiceDefaultsLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.ServiceDefaultsAssembly).As("Service Defaults Layer");

    protected IObjectProvider<IType> UnitTestsLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.UnitTestsAssembly).As("Unit Tests Layer");

    protected IObjectProvider<IType> WorkerCommonLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.WorkerCommonAssembly).As("Worker Common Layer");

    protected IObjectProvider<IType> WorkerMailSenderLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.WorkerMailSenderAssembly).As("Worker Mail Sender Layer");

    protected IObjectProvider<IType> WorkerMessagesHandlerLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.WorkerMessagesHandlerAssembly).As("Worker Messages Handler Layer");

    protected IObjectProvider<IType> WorkerDbInitializerLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.WorkerDbInitializerAssembly).As("Worker DB Initializer Layer");

    protected IObjectProvider<IType> ArchitectureTestsLayer { get; private set; } = Types().That().ResideInAssembly(architectureBuilder.ArchitectureTestsAssembly).As("Architecture Tests Layer");

    protected static IEnumerable<IArchRule> GetForbiddenDependenciesRules(IEnumerable<string> testedTypes, IEnumerable<string> forbiddenTypes, string ruleDescription)
    {
        return testedTypes.SelectMany(testedType =>
            forbiddenTypes.Select(forbiddenType =>
                Types().That()
                    .ResideInNamespace(testedType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(forbiddenType)
                    .As(string.Format(ruleDescription, testedType, forbiddenType))
            )
        );
    }
}
