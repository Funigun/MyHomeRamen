using MyHomeRamen.ArchitectureTests.Common;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class MenuModuleBoundriesTests : BaseArchitectureTest
{
    [Fact]
    public void MenuModule_Should_Not_Access_Other_Modules_Directly()
    {
        // Arrange
        string[]? forbiddenModules = ["MyHomeRamen.Domain.Orders", "MyHomeRamen.Domain.Payments",
                                             "MyHomeRamen.Domain.Reservations", "MyHomeRamen.Domain.Basket"];

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .That()
                                 .ResideInNamespace("MyHomeRamen.Domain.Menu")
                                 .ShouldNot()
                                 .HaveDependencyOnAny(forbiddenModules)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Menu module should not access other modules directly.");
    }
}
