using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Users;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void UsersApi_ShouldNot_DependOn_MenuApi()
    {
        // Arrange
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, menuApi, "Users API type '{0}' should not depend on Menu API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersApi_ShouldNot_DependOn_OrdersApi()
    {
        // Arrange
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, ordersApi, "Users API type '{0}' should not depend on Orders API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersApi_ShouldNot_DependOn_PaymentsApi()
    {
        // Arrange
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, paymentsApi, "Users API type '{0}' should not depend on Payments API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersApi_ShouldNot_DependOn_ReservationsApi()
    {
        // Arrange
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, reservationsApi, "Users API type '{0}' should not depend on Reservations API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersApi_ShouldNot_DependOn_ShoppingCartApi()
    {
        // Arrange
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, shoppingCartApi, "Users API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
