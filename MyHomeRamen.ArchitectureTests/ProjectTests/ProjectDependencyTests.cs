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
            architectureBuilder.ApiAssembly,
            architectureBuilder.BlazorServerAssembly,
            architectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> appHostRules = PrepareProjectRules(architectureBuilder.AppHostAssembly, allowedAssemblies);

        foreach (IArchRule rule in appHostRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void BlazorServer_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.BlazorClientAssembly,
            architectureBuilder.ServiceDefaultsAssembly,
            architectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> blazorServerRules = PrepareProjectRules(architectureBuilder.BlazorServerAssembly, allowedAssemblies);

        foreach (IArchRule rule in blazorServerRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void IdentityApi_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.ApiCommonAssembly,
            architectureBuilder.DomainAssembly,
            architectureBuilder.InfrastructureAssembly,
            architectureBuilder.PersistanceAssembly,
            architectureBuilder.ServiceDefaultsAssembly,
            architectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> identityApiRules = PrepareProjectRules(architectureBuilder.IdentityApiAssembly, allowedAssemblies);

        foreach (IArchRule rule in identityApiRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Api_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.ApiCommonAssembly,
            architectureBuilder.DomainAssembly,
            architectureBuilder.InfrastructureAssembly,
            architectureBuilder.PersistanceAssembly,
            architectureBuilder.ServiceDefaultsAssembly,
            architectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> apiRules = PrepareProjectRules(architectureBuilder.ApiAssembly, allowedAssemblies);

        foreach (IArchRule rule in apiRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Infrastructure_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.DomainAssembly,
            architectureBuilder.ApiCommonAssembly
        ];

        IEnumerable<IArchRule> infrastructureRules = PrepareProjectRules(architectureBuilder.InfrastructureAssembly, allowedAssemblies);

        foreach (IArchRule rule in infrastructureRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Persistance_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.DomainAssembly,
            architectureBuilder.ApiCommonAssembly
        ];

        IEnumerable<IArchRule> persistanceRules = PrepareProjectRules(architectureBuilder.PersistanceAssembly, allowedAssemblies);

        foreach (IArchRule rule in persistanceRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerMailSender_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.WorkerCommonAssembly,
            architectureBuilder.DomainAssembly,
            architectureBuilder.InfrastructureAssembly,
            architectureBuilder.PersistanceAssembly,
            architectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> workerMailSenderRules = PrepareProjectRules(architectureBuilder.WorkerMailSenderAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerMailSenderRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerMessagesHandler_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.ApiCommonAssembly,
            architectureBuilder.WorkerCommonAssembly,
            architectureBuilder.DomainAssembly,
            architectureBuilder.InfrastructureAssembly,
            architectureBuilder.PersistanceAssembly,
            architectureBuilder.ServiceDefaultsAssembly,
            architectureBuilder.ApiContractsAssembly
        ];

        IEnumerable<IArchRule> workerMessagesHandlerRules = PrepareProjectRules(architectureBuilder.WorkerMessagesHandlerAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerMessagesHandlerRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void WorkerDbInitializer_ShouldDepend_OnlyOnAllowedAssemblies()
    {
        IEnumerable<Assembly> allowedAssemblies =
        [
            architectureBuilder.ApiCommonAssembly,
            architectureBuilder.WorkerCommonAssembly,
            architectureBuilder.DomainAssembly,
            architectureBuilder.InfrastructureAssembly,
            architectureBuilder.PersistanceAssembly,
            architectureBuilder.ServiceDefaultsAssembly
        ];

        IEnumerable<IArchRule> workerDbInitializerRules = PrepareProjectRules(architectureBuilder.WorkerDbInitializerAssembly, allowedAssemblies);

        foreach (IArchRule rule in workerDbInitializerRules)
        {
            rule.Check(architectureBuilder.Architecture);
        }
    }

    private IEnumerable<IArchRule> PrepareProjectRules(Assembly projectAssembly, IEnumerable<Assembly> allowedDependencies)
    {
        IEnumerable<Assembly> forbiddenAssemblies = architectureBuilder.AllAssemblies
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
        Assembly[] commonAssemblies = [architectureBuilder.WorkerCommonAssembly, architectureBuilder.ServiceDefaultsAssembly, architectureBuilder.ApiCommonAssembly];

        IEnumerable<Assembly> otherProjectAssemblies = architectureBuilder.AllAssemblies
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
            rule.Check(architectureBuilder.Architecture);
        }
    }

    [Fact]
    public void Domain_Should_OnlyUse_ApiCommonDomain_Namespace()
    {
        // Arrange
        // Forbidden = all Api.Common types that are NOT in the allowed sub-namespaces (Domain, Exceptions)
        IObjectProvider<IType> forbiddenApiCommonTypes = Types().That()
            .ResideInNamespace("MyHomeRamen.Api.Common")
            .And()
            .DoNotResideInNamespace("MyHomeRamen.Api.Common.Domain")
            .And()
            .DoNotResideInNamespace("MyHomeRamen.Api.Common.Exceptions")
            .As("Forbidden Api.Common types");

        // Act
        List<EvaluationResult> failures = [..((IArchRule)Classes().That()
                     .Are(DomainLayer)
                     .Should()
                     .NotDependOnAnyTypesThat()
                     .Are(forbiddenApiCommonTypes)).Evaluate(architectureBuilder.Architecture).Where(r => !r.Passed)];

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
                     .Are(forbiddenApiCommonTypes).Check(architectureBuilder.Architecture);
    }
}
