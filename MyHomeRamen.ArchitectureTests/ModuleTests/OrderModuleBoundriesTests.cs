using MyHomeRamen.ArchitectureTests.Common;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class OrderModuleBoundriesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void OrderModule_Should_Not_Access_Other_Modules_Directly()
    {
        // Arrange
        string[]? forbiddenModules = ["MyHomeRamen.Domain.Menu", "MyHomeRamen.Domain.Payments",
                                             "MyHomeRamen.Domain.Reservations", "MyHomeRamen.Domain.ShoppingCart",
                                             "MyHomeRamen.Identity.Api", "MyHomeRamen.Worker"];

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .That()
                                 .ResideInNamespace("MyHomeRamen.Domain.Orders")
                                 .ShouldNot()
                                 .HaveDependencyOnAny(forbiddenModules)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "Order module should not access other modules directly.");
    }
}
