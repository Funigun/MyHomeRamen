using MyHomeRamen.ArchitectureTests.Common;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class BasketModuleBoundriesTests : BaseArchitectureTest
{
    [Fact]
    public void BasketModule_Should_Not_Access_Other_Modules_Directly()
    {
        // Arrange
        string[]? forbiddenModules = ["MyHomeRamen.Domain.Menu", "MyHomeRamen.Domain.Orders",
                                             "MyHomeRamen.Domain.Payments", "MyHomeRamen.Domain.Reservations",
                                             "MyHomeRamen.Identity.Api", "MyHomeRamen.Worker"];

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .That()
                                 .ResideInNamespace("MyHomeRamen.Domain.ShoppingCart")
                                 .ShouldNot()
                                 .HaveDependencyOnAny(forbiddenModules)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Payments module should not access other modules directly.");
    }
}
