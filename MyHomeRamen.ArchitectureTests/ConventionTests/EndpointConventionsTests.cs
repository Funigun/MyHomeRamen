using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Query;
using Xunit.Sdk;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ConventionTests;

public sealed class EndpointConventionsTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void EndpointImplementations_ShouldHave_EnpointSuffix()
    {
        // Arrange
        IArchRule rule = Types()
                    .That()
                    .ImplementInterface(typeof(IEndpoint))
                    .And()
                    .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                    .Should()
                    .HaveNameEndingWith("Endpoint");

        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void EndpointImplementations_ShouldNot_UseCommandsDirectlyInParameters()
    {
        // Arrange
        IEnumerable<Type> endpointTypes = ArchitectureBuilder.ApiAssembly
                                                             .GetTypes()
                                                             .Where(t => t.Implements(typeof(IEndpoint)) && t.IsPublic && !t.IsInterface && !t.IsAbstract);

        foreach (Type endpointType in endpointTypes)
        {
            IEnumerable<System.Reflection.MethodInfo>? methods = endpointType.GetMethods()
                                      .Where(m => m.IsPublic && !m.IsStatic);
            foreach (System.Reflection.MethodInfo method in methods)
            {
                System.Reflection.ParameterInfo[]? parameters = method.GetParameters();
                foreach (System.Reflection.ParameterInfo parameter in parameters)
                {
                    if (parameter.ParameterType.IsGenericType)
                    {
                        Type parameterDefinition = parameter.ParameterType.GetGenericTypeDefinition();

                        if (parameterDefinition == typeof(ICommand<>) || parameterDefinition == typeof(ICommand))
                        {
                            throw new XunitException($"Endpoint '{endpointType.FullName}' has a method '{method.Name}' that uses ICommand directly in its parameters. Endpoints should not depend on commands directly.");
                        }

                        if (parameterDefinition == typeof(IQuery<>))
                        {
                            throw new XunitException($"Endpoint '{endpointType.FullName}' has a method '{method.Name}' that uses IQuery directly in its parameters. Endpoints should not depend on queries directly.");
                        }
                    }
                }
            }
        }
    }
}
