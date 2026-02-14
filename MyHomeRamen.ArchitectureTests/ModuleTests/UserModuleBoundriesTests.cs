using MyHomeRamen.ArchitectureTests.Common;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class UserModuleBoundriesTests : BaseArchitectureTest
{
    [Fact]
    public void UserModule_Should_Not_Access_Other_Modules_Directly()
    {
        // Arrange
        string[]? forbiddenModules = ["MyHomeRamen.Domain.Menu", "MyHomeRamen.Domain.Orders",
                                             "MyHomeRamen.Domain.Payments", "MyHomeRamen.Domain.Reservations",
                                             "MyHomeRamen.Domain.ShoppingCart", "MyHomeRamen.Worker"];

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .That()
                                 .ResideInNamespace("MyHomeRamen.Identity.Api")
                                 .ShouldNot()
                                 .HaveDependencyOnAny(forbiddenModules)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "User module should not access other modules directly.");
    }

    [Fact]
    public void UserModule_ShouldAccess_Domain_OnlyFor_UserRelated_Functionality()
    {
        // Arrange
        string[]? allowedNamespaces =
        [
            "MyHomeRamen.Domain.Users",
            "MyHomeRamen.Domain.Common.Authorization",
            "MyHomeRamen.Domain.Menu.Authorization",
            "MyHomeRamen.Domain.Orders.Authorization",
            "MyHomeRamen.Domain.Payments.Authorization",
            "MyHomeRamen.Domain.Reservations.Authorization",
            "MyHomeRamen.Domain.ShoppingCart.Authorization"
        ];

        // Act
        TestResult result = Types.InAssembly(DomainAssembly)
                                 .That()
                                 .ResideInNamespace("MyHomeRamen.Identity.Api")
                                 .Should()
                                 .OnlyHaveDependenciesOn(allowedNamespaces)
                                 .GetResult();

        // Assert
        Assert.True(result.IsSuccessful, "User module should only access user-related functionality in the domain.");
    }
}
