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
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");

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
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");

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
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");

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
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");

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
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersApi, shoppingCartApi, "Users API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
