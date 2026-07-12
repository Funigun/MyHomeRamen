using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Menu;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void MenuApi_ShouldNot_DependOn_OrdersApi()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, ordersApi, "Menu API type '{0}' should not depend on Orders API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldNot_DependOn_PaymentsApi()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, paymentsApi, "Menu API type '{0}' should not depend on Payments API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldNot_DependOn_ReservationsApi()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, reservationsApi, "Menu API type '{0}' should not depend on Reservations API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldNot_DependOn_ShoppingCartApi()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, shoppingCartApi, "Menu API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldNot_DependOn_UsersApi()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, usersApi, "Menu API type '{0}' should not depend on Users API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldDepend_OnlyOn_MenuDomain()
    {
        // Arrange
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Menu", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, otherDomains, "Menu API type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuApi_ShouldDepend_OnlyOn_MenuPersistance()
    {
        // Arrange
        IEnumerable<string> allowedPersistanceNamespaces =
        [
            "MyHomeRamen.Persistance.Menu",
            "MyHomeRamen.Persistance.Common",
        ];

        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");
        IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance")
            .Where(name => name != "MyHomeRamen.Persistance"
                        && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuApi, forbiddenPersistence, "Menu API type '{0}' should not depend on persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void GetProductsByCategoryEndpoint_ShouldNotDependOn_AuthorizationPolicies()
    {
        // Arrange
        IEnumerable<string> endpointTypes = ArchitectureBuilder.ApiFeaturesAssembly
            .TypesInNamespace("MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory");

        IEnumerable<string> authPolicyTypes = ArchitectureBuilder.ApiFeaturesAssembly
            .TypesInNamespace("MyHomeRamen.Features.WebPresentation");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            endpointTypes,
            authPolicyTypes,
            "Anonymous endpoint type '{0}' should not depend on authorization policy type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
