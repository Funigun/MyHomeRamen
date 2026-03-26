using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;
using Assembly = System.Reflection.Assembly;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests;

public sealed class ProjectDependencyTests(ITestOutputHelper outputHelper, ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    public static TheoryData<Assembly, Assembly[]> GetProjectDependencies()
    {
        TheoryData<Assembly, Assembly[]> data = new()
        {
            { BlazorServerAssembly, new[] { BlazorClientAssembly, ServiceDefaultsAssembly, ApiContractsAssembly } },
            { AppHostAssembly, new[] { ApiAssembly, BlazorServerAssembly, ServiceDefaultsAssembly } },
            { IdentityApiAssembly, new[] { ApiCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly, ApiContractsAssembly } },
            { ApiAssembly, new[] { ApiCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly, ApiContractsAssembly } },
            { InfrastructureAssembly, new[] { DomainAssembly, ApiCommonAssembly } },
            { PersistanceAssembly, new[] { DomainAssembly, ApiCommonAssembly } },
            { WorkerMailSenderAssembly, new[] { WorkerCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } },
            { WorkerMessagesHandlerAssembly, new[] { ApiCommonAssembly, WorkerCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly, ApiContractsAssembly } },
            { WorkerDbInitializerAssembly, new[] { ApiCommonAssembly, WorkerCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } }
        };
        return data;
    }

    [Theory]
#pragma warning disable xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    [MemberData(nameof(GetProjectDependencies))]
#pragma warning restore xUnit1045 // Avoid using TheoryData type arguments that might not be serializable
    public void Projects_ShouldHave_OnlyAllowedDependencies(Assembly projectAssembly, Assembly[] allowedDependencies)
    {
        // Act
        bool hasOnlyAllowedDependencies = projectAssembly.GetReferencedAssemblies()
                                                     .Where(r => r.FullName.StartsWith("MyHomeRamen"))
                                                     .All(a => allowedDependencies.Any(allowed => allowed.FullName == a.FullName));

        // Assert
        Assert.True(hasOnlyAllowedDependencies, $"{projectAssembly.GetName().Name} should not have project dependencies other than {string.Join("\n", allowedDependencies.Select(d => d.GetName()))}");
    }

    [Fact]
    public void CommonProjects_ShouldNotHave_AnyProjectDependencies()
    {
        // Arrange
        Assembly[] commonProjects = [WorkerCommonAssembly, ServiceDefaultsAssembly, ApiCommonAssembly];

        foreach (Assembly commonProject in commonProjects)
        {
            // Act
            bool hasProjectDependencies = commonProject.GetReferencedAssemblies()
                                                       .Any(a => ProjectAssemblies.Any(pa => pa.FullName == a.FullName));

            // Assert
            Assert.False(hasProjectDependencies, $"{commonProject.GetName().Name} should not have any project dependencies");
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

        IArchRule domainShouldNotUseForbiddenApiCommon =
            Classes().That()
                     .Are(DomainLayer)
                     .Should()
                     .NotDependOnAnyTypesThat()
                     .Are(forbiddenApiCommonTypes);

        // Act
        List<EvaluationResult> failures = [..domainShouldNotUseForbiddenApiCommon.Evaluate(Architecture).Where(r => !r.Passed)];

        if (failures.Count != 0)
        {
            outputHelper.WriteLine("The following types in the Domain assembly have forbidden dependencies:");

            foreach (EvaluationResult failure in failures)
            {
                outputHelper.WriteLine($"- {failure.Description}");
            }
        }

        // Assert
        domainShouldNotUseForbiddenApiCommon.Check(Architecture);
    }
}
