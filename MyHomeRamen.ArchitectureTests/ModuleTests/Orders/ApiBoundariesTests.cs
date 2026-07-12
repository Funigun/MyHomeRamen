using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Orders;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void OrdersApi_ShouldNot_DependOn_MenuApi()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, menuApi, "Orders API type '{0}' should not depend on Menu API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldNot_DependOn_PaymentsApi()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, paymentsApi, "Orders API type '{0}' should not depend on Payments API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldNot_DependOn_ReservationsApi()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, reservationsApi, "Orders API type '{0}' should not depend on Reservations API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldNot_DependOn_ShoppingCartApi()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, shoppingCartApi, "Orders API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldNot_DependOn_UsersApi()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, usersApi, "Orders API type '{0}' should not depend on Identity API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldDepend_OnlyOn_OrdersDomain()
    {
        // Arrange
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Orders", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, otherDomains, "Orders API type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void OrdersApi_ShouldDepend_OnlyOn_OrdersPersistance()
    {
        // Arrange
        IEnumerable<string> allowedPersistanceNamespaces =
        [
            "MyHomeRamen.Persistance.Orders",
            "MyHomeRamen.Persistance.Common",
        ];

        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");
        IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance")
            .Where(name => name != "MyHomeRamen.Persistance"
                        && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(ordersApi, forbiddenPersistence, "Orders API type '{0}' should not depend on persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
