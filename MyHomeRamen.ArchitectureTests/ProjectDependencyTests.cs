using System.Reflection;
using MyHomeRamen.ArchitectureTests.Common;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace MyHomeRamen.ArchitectureTests;

public sealed class ProjectDependencyTests : BaseArchitectureTest
{
    public static TheoryData<Assembly, Assembly[]> GetProjectDependencies()
    {
        TheoryData<Assembly, Assembly[]> data = new()
        {
            { BlazorServerAssembly, new[] { BlazorClientAssembly, ServiceDefaultsAssembly } },
            { AppHostAssembly, new[] { ApiAssembly, BlazorServerAssembly, ServiceDefaultsAssembly } },
            { IdentityApiAssembly, new[] { ApiCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } },
            { ApiAssembly, new[] { ApiCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } },
            { InfrastructureAssembly, new[] { DomainAssembly, ServiceDefaultsAssembly } },
            { PersistanceAssembly, new[] { DomainAssembly, ServiceDefaultsAssembly } },
            { WorkerMailSenderAssembly, new[] { WorkerCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } },
            { WorkerMessagesHandlerAssembly, new[] { WorkerCommonAssembly, DomainAssembly, InfrastructureAssembly, PersistanceAssembly, ServiceDefaultsAssembly } }
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
        Assert.True(hasOnlyAllowedDependencies, $"{projectAssembly.GetName().Name} should not have project dependencies other than {string.Join(",", allowedDependencies.Select(d => d.GetName()))}");
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
        string allowedNamespace = "MyHomeRamen.Api.Common.Domain";
        Type[] apiCommonTypes = ApiCommonAssembly.GetTypes();

        string[] forbiddenNamespacesOrTypes = apiCommonTypes
            .Where(t => t.FullName != null && t.Namespace != null && t.Namespace.StartsWith("MyHomeRamen.Api.Common"))
            .Select(t =>
            {
                // If the type is in the allowed namespace or a child of it, it is not forbidden.
                if (t.Namespace == allowedNamespace || t.Namespace!.StartsWith($"{allowedNamespace}."))
                {
                    return null;
                }

                // If the type is in the root namespace "MyHomeRamen.Api.Common", we must block the Type specifically
                // because blocking the namespace blocks the allowed child "Domain".
                if (t.Namespace == "MyHomeRamen.Api.Common")
                {
                    return t.FullName;
                }

                // For sibling namespaces (e.g. .Middleware), we can block the namespace.
                return t.Namespace;
            })
            .Where(x => x != null)
            .Distinct()
            .ToArray()!;

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .Should()
                                 .NotHaveDependencyOnAny(forbiddenNamespacesOrTypes)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Domain assembly should only use items from 'MyHomeRamen.Api.Common.Domain' namespaces, but no others from Api.Common.");
    }
}
