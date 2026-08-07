using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;
using Assembly = System.Reflection.Assembly;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ProjectTests;

public sealed class ProjectDependencyTests(ITestOutputHelper outputHelper, ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void AppHost_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.ApiFeaturesAssembly,
            ArchitectureBuilder.BlazorServerAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> appHostRules = PrepareProjectRules(ArchitectureBuilder.AppHostAssembly, allowedAssemblies);

        foreach (IArchRule rule in appHostRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void BlazorServer_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.BlazorClientAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly,
            ArchitectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> blazorServerRules = PrepareProjectRules(ArchitectureBuilder.BlazorServerAssembly, allowedAssemblies);

        foreach (IArchRule rule in blazorServerRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Api_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.ApiFeaturesAssembly,
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.InfrastructureAssembly,
            ArchitectureBuilder.PersistanceAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly,
            ArchitectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> apiRules = PrepareProjectRules(ArchitectureBuilder.ApiFeaturesAssembly, allowedAssemblies);

        foreach (IArchRule rule in apiRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Infrastructure_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.ApiFeaturesAssembly
        ];

        IEnumerable<IArchRule> infrastructureRules = PrepareProjectRules(ArchitectureBuilder.InfrastructureAssembly, allowedAssemblies);

        foreach (IArchRule rule in infrastructureRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Persistance_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.ApiFeaturesAssembly,
            ArchitectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> persistanceRules = PrepareProjectRules(ArchitectureBuilder.PersistanceAssembly, allowedAssemblies);

        foreach (IArchRule rule in persistanceRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerMailSender_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.WorkerCommonAssembly,
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.InfrastructureAssembly,
            ArchitectureBuilder.PersistanceAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> workerMailSenderRules = PrepareProjectRules(ArchitectureBuilder.WorkerMailSenderAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerMailSenderRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerMessagesHandler_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.ApiFeaturesAssembly,
            ArchitectureBuilder.WorkerCommonAssembly,
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.InfrastructureAssembly,
            ArchitectureBuilder.PersistanceAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly,
            ArchitectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> workerMessagesHandlerRules = PrepareProjectRules(ArchitectureBuilder.WorkerMessagesHandlerAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerMessagesHandlerRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerDbInitializer_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            ArchitectureBuilder.ApiFeaturesAssembly,
            ArchitectureBuilder.WorkerCommonAssembly,
            ArchitectureBuilder.DomainAssembly,
            ArchitectureBuilder.InfrastructureAssembly,
            ArchitectureBuilder.PersistanceAssembly,
            ArchitectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> workerDbInitializerRules = PrepareProjectRules(ArchitectureBuilder.WorkerDbInitializerAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerDbInitializerRules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    private IEnumerable<IArchRule> PrepareProjectRules(Assembly projectAssembly, IEnumerable<Assembly> allowedDependencies)
    {
        IEnumerable<Assembly> forbiddenAssemblies = ArchitectureBuilder.AllAssemblies
            .Where(a => a.FullName != projectAssembly.FullName && !allowedDependencies.Any(allowed => allowed.FullName == a.FullName));
        return forbiddenAssemblies.Select(forbidden =>
            Types().That()
                .ResideInAssembly(projectAssembly)
                .Should()
                .NotDependOnAnyTypesThat()
                .ResideInAssembly(forbidden)
                .As($"Types in '{projectAssembly.GetName().Name}' should not depend on types from '{forbidden.GetName().Name}'")
        );
    }

    [Fact]
    public void CommonProjects_ShouldNotHave_AnyProjectDependencies()
    {
        // Arrange
        Assembly[] commonAssemblies = [ArchitectureBuilder.WorkerCommonAssembly, ArchitectureBuilder.ServiceDefaultsAssembly];

        IEnumerable<Assembly> otherProjectAssemblies = ArchitectureBuilder.AllAssemblies
            .Where(a => !commonAssemblies.Any(c => c.FullName == a.FullName));

        IEnumerable<IArchRule> rules = commonAssemblies.SelectMany(commonAssembly =>
            otherProjectAssemblies.Select(projectAssembly =>
                Types().That()
                    .ResideInAssembly(commonAssembly)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInAssembly(projectAssembly)
                    .As($"'{commonAssembly.GetName().Name}' should not depend on '{projectAssembly.GetName().Name}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Domain_Should_OnlyUse_ApiCommonDomain_Namespace()
    {
        // Arrange
        // Forbidden = all Api.Common types that are NOT in the allowed sub-namespaces (Domain, Exceptions)
        IObjectProvider<IType> forbiddenApiCommonTypes = Types().That()
            .ResideInNamespace("MyHomeRamen.Features.Common")
            .And()
            .DoNotResideInNamespace("MyHomeRamen.Features.Common.Domain")
            .And()
            .DoNotResideInNamespace("MyHomeRamen.Features.Common.Exceptions")
            .As("Forbidden Api.Common types");

        // Act
        List<EvaluationResult> failures = [..((IArchRule)Classes().That()
                     .Are(DomainLayer)
                     .Should()
                     .NotDependOnAnyTypesThat()
                     .Are(forbiddenApiCommonTypes)).Evaluate(ArchitectureBuilder.Architecture).Where(r => !r.Passed)];

        if (failures.Count != 0)
        {
            outputHelper.WriteLine("The following types in the Domain assembly have forbidden dependencies:");

            foreach (EvaluationResult failure in failures)
            {
                outputHelper.WriteLine($"- {failure.Description}");
            }
        }

        // Assert
        Classes().That()
                     .Are(DomainLayer)
                     .Should()
                     .NotDependOnAnyTypesThat()
                     .Are(forbiddenApiCommonTypes).Check(ArchitectureBuilder.Architecture);
    }
}
