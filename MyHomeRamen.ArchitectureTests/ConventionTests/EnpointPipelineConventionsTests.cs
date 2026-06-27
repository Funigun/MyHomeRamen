using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints.Query;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ConventionTests;

public sealed class EnpointPipelineConventionsTests(ArchitectureBuilder architectureBuilder, ITestOutputHelper testOutputHelper) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void QueryImplementations_ShouldEndWith_QuerySuffix()
    {
        // Arrange
        IArchRule rule = Types()
                            .That()
                            .ImplementInterface(typeof(IQuery<>))
                            .And()
                            .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                            .Should()
                            .HaveNameEndingWith("Query");

        // Act & Assert
        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void QueryImplementations_ShouldHave_ProperQueryHandlerImplementation()
    {
        // Arrange
        System.Reflection.Assembly apiAssembly = ArchitectureBuilder.ApiAssembly;
        Type[] queryImplementations = apiAssembly.GetTypes()
            .Where(t => t.IsPublic && typeof(IQuery<>).IsAssignableFrom(t) && !t.IsInterface && !t.IsGenericTypeDefinition)
            .ToArray();

        // Act & Assert
        foreach (Type queryType in queryImplementations)
        {
            // Get the response type from IQuery<TResponse>
            Type? queryInterface = queryType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

            if (queryInterface is not null)
            {
                Type responseType = queryInterface.GetGenericArguments()[0];

                // Check if there's a corresponding handler
                Type expectedHandlerInterface = typeof(IQueryHandler<,>).MakeGenericType(queryType, responseType);

                Type? handlerImplementation = apiAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsPublic && !t.IsInterface && expectedHandlerInterface.IsAssignableFrom(t));

                Assert.NotNull(handlerImplementation);
            }
        }
    }

    [Fact]
    public void QueryHandlerImplementations_ShouldEndWith_HandlerSuffix()
    {
        // Arrange
        IArchRule rule = Types()
                            .That()
                            .ImplementInterface(typeof(IQueryHandler<,>))
                            .And()
                            .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                            .Should()
                            .HaveNameEndingWith("Handler");

        // Act & Assert
        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void CommandImplementations_ShouldEndWith_CommandSuffix()
    {
        // Arrange
        IArchRule rule = Types()
                            .That()
                            .ImplementInterface(typeof(ICommand<>))
                            .And()
                            .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                            .Should()
                            .HaveNameEndingWith("Command");

        // Act & Assert
        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void CommandImplementations_ShouldHave_ProperCommandHandlerImplementation()
    {
        // Arrange
        System.Reflection.Assembly apiAssembly = ArchitectureBuilder.ApiAssembly;
        Type[] commandImplementations = apiAssembly.GetTypes()
            .Where(t => t.IsPublic && typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.Equals(typeof(ICommand)))
            .ToArray();

        bool anyCommandWithoutHandler = false;

        // Act & Assert
        foreach (Type commandType in commandImplementations)
        {
            // Get the response type from ICommand<TResponse> if it's generic
            Type? commandInterface = commandType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

            if (commandInterface is not null)
            {
                Type responseType = commandInterface.GetGenericArguments()[0];

                // Check if there's a corresponding handler
                Type expectedHandlerInterface = typeof(ICommandHandler<,>).MakeGenericType(commandType, responseType);

                Type? handlerImplementation = apiAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsPublic && !t.IsInterface && expectedHandlerInterface.IsAssignableFrom(t));

                if (handlerImplementation is null)
                {
                    testOutputHelper.WriteLine($"{commandType.FullName} does not have a corresponding handler.");
                    anyCommandWithoutHandler = true;
                }
            }
        }

        // Assert
        Assert.False(anyCommandWithoutHandler);
    }

    [Fact]
    public void CommandHandlerImplementations_ShouldEndWith_HandlerSuffix()
    {
        // Arrange
        IArchRule handlerRule = Types()
                                    .That()
                                    .ImplementInterface(typeof(ICommandHandler<>))
                                    .And()
                                    .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                                    .Should()
                                    .HaveNameEndingWith("Handler");

        IArchRule handlerWithResponseRule = Types()
                                                .That()
                                                .ImplementInterface(typeof(ICommandHandler<,>))
                                                .And()
                                                .ResideInAssembly(ArchitectureBuilder.ApiAssembly)
                                                .Should()
                                                .HaveNameEndingWith("Handler");

        // Act & Assert
        handlerRule.Check(ArchitectureBuilder.Architecture);
        handlerWithResponseRule.Check(ArchitectureBuilder.Architecture);
    }
}
