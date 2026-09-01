using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;
using MyHomeRamen.Features.Common.Mediator;
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
                            .ResideInAssembly(ArchitectureBuilder.ApiFeaturesAssembly)
                            .Should()
                            .HaveNameEndingWith("Query");

        // Act & Assert
        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void QueryImplementations_ShouldHave_ProperQueryHandlerImplementation()
    {
        // Arrange
        System.Reflection.Assembly apiAssembly = ArchitectureBuilder.ApiFeaturesAssembly;
        Type[] queryImplementations = apiAssembly.GetTypes()
            .Where(t => t.IsPublic && ImplementsGenericInterface(t, typeof(IQuery<>)) && !t.IsInterface && !t.IsGenericTypeDefinition)
            .ToArray();

        // Act & Assert
        foreach (Type queryType in queryImplementations)
        {
            Type? queryInterface = queryType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

            if (queryInterface is not null)
            {
                Type responseType = queryInterface.GetGenericArguments()[0];
                Type expectedHandlerInterface = typeof(IRequestHandler<,>).MakeGenericType(queryType, responseType);

                Type? handlerImplementation = apiAssembly.GetTypes()
                    .FirstOrDefault(t => t.IsPublic && !t.IsInterface && expectedHandlerInterface.IsAssignableFrom(t));

                Assert.NotNull(handlerImplementation);
            }
        }
    }

    [Fact]
    public void QueryHandlerImplementations_ShouldEndWith_HandlerSuffix()
    {
        Type[] handlerImplementations = ArchitectureBuilder.ApiFeaturesAssembly
            .GetTypes()
            .Where(t => t.IsPublic
                && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    && ImplementsGenericInterface(i.GetGenericArguments()[0], typeof(IQuery<>))))
            .ToArray();

        Assert.All(handlerImplementations, handlerImplementation =>
            Assert.True(
                GetTypeNameWithoutGenericArity(handlerImplementation).EndsWith("Handler", StringComparison.Ordinal),
                $"Query handler implementation '{handlerImplementation.FullName}' should end with 'Handler'."));
    }

    [Fact]
    public void CommandImplementations_ShouldEndWith_CommandSuffix()
    {
        // Arrange
        IArchRule rule = Types()
                            .That()
                            .ImplementInterface(typeof(ICommand<>))
                            .And()
                            .ResideInAssembly(ArchitectureBuilder.ApiFeaturesAssembly)
                            .Should()
                            .HaveNameEndingWith("Command");

        // Act & Assert
        rule.Check(ArchitectureBuilder.Architecture);
    }

    [Fact]
    public void CommandImplementations_ShouldHave_ProperCommandHandlerImplementation()
    {
        // Arrange
        System.Reflection.Assembly apiAssembly = ArchitectureBuilder.ApiFeaturesAssembly;
        Type[] commandImplementations = apiAssembly.GetTypes()
            .Where(t => t.IsPublic
                && (typeof(ICommand).IsAssignableFrom(t) || ImplementsGenericInterface(t, typeof(ICommand<>)))
                && !t.IsInterface
                && !t.Equals(typeof(ICommand)))
            .ToArray();

        bool anyCommandWithoutHandler = false;

        // Act & Assert
        foreach (Type commandType in commandImplementations)
        {
            Type? commandInterface = commandType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
            Type responseType = commandInterface?.GetGenericArguments()[0] ?? typeof(Unit);
            Type expectedHandlerInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, responseType);

            Type? handlerImplementation = apiAssembly.GetTypes()
                .FirstOrDefault(t => t.IsPublic && !t.IsInterface && expectedHandlerInterface.IsAssignableFrom(t));

            if (handlerImplementation is null)
            {
                testOutputHelper.WriteLine($"{commandType.FullName} does not have a corresponding handler.");
                anyCommandWithoutHandler = true;
            }
        }

        // Assert
        Assert.False(anyCommandWithoutHandler);
    }

    [Fact]
    public void CommandHandlerImplementations_ShouldEndWith_HandlerSuffix()
    {
        Type[] handlerImplementations = ArchitectureBuilder.ApiFeaturesAssembly
            .GetTypes()
            .Where(t => t.IsPublic
                && !t.IsInterface
                && t.GetInterfaces().Any(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    && IsCommandType(i.GetGenericArguments()[0])))
            .ToArray();

        Assert.All(handlerImplementations, handlerImplementation =>
            Assert.True(
                GetTypeNameWithoutGenericArity(handlerImplementation).EndsWith("Handler", StringComparison.Ordinal),
                $"Command handler implementation '{handlerImplementation.FullName}' should end with 'Handler'."));
    }

    private static bool IsCommandType(Type type)
    {
        return typeof(ICommand).IsAssignableFrom(type) || ImplementsGenericInterface(type, typeof(ICommand<>));
    }

    private static bool ImplementsGenericInterface(Type type, Type genericInterface)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface);
    }

    private static string GetTypeNameWithoutGenericArity(Type type)
    {
        string name = type.Name;
        int arityIndex = name.IndexOf('`', StringComparison.OrdinalIgnoreCase);
        return arityIndex >= 0 ? name[..arityIndex] : name;
    }
}
