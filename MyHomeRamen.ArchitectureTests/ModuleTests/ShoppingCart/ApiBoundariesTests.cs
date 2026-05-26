using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.ShoppingCart;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void ShoppingCartApi_ShouldNot_DependOn_MenuApi()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, menuApi, "ShoppingCart API type '{0}' should not depend on Menu API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldNot_DependOn_OrdersApi()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, ordersApi, "ShoppingCart API type '{0}' should not depend on Orders API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldNot_DependOn_PaymentsApi()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, paymentsApi, "ShoppingCart API type '{0}' should not depend on Payments API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldNot_DependOn_ReservationsApi()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, reservationsApi, "ShoppingCart API type '{0}' should not depend on Reservations API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldNot_DependOn_UsersApi()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Users");
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, usersApi, "ShoppingCart API type '{0}' should not depend on Users API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldDepend_OnlyOn_ShoppingCartDomain()
    {
        // Arrange
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.ShoppingCart", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, otherDomains, "ShoppingCart API type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartApi_ShouldDepend_OnlyOn_ShoppingCartPersistance()
    {
        // Arrange
        IEnumerable<string> allowedPersistanceNamespaces =
        [
            "MyHomeRamen.Persistance.ShoppingCart",
            "MyHomeRamen.Persistance.Common",
        ];

        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.ShoppingCart");
        IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance")
            .Where(name => name != "MyHomeRamen.Persistance"
                        && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(shoppingCartApi, forbiddenPersistence, "ShoppingCart API type '{0}' should not depend on persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
