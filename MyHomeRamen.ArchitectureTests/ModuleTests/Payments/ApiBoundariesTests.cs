using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Payments;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void PaymentsApi_ShouldNot_DependOn_MenuApi()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, menuApi, "Payments API type '{0}' should not depend on Menu API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldNot_DependOn_OrdersApi()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, ordersApi, "Payments API type '{0}' should not depend on Orders API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldNot_DependOn_ReservationsApi()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, reservationsApi, "Payments API type '{0}' should not depend on Reservations API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldNot_DependOn_ShoppingCartApi()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, shoppingCartApi, "Payments API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldNot_DependOn_UsersApi()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, usersApi, "Payments API type '{0}' should not depend on Users API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldDepend_OnlyOn_PaymentsDomain()
    {
        // Arrange
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Payments", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, otherDomains, "Payments API type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsApi_ShouldDepend_OnlyOn_PaymentsPersistance()
    {
        // Arrange
        IEnumerable<string> allowedPersistanceNamespaces =
        [
            "MyHomeRamen.Persistance.Payments",
            "MyHomeRamen.Persistance.Common",
        ];

        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");
        IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance")
            .Where(name => name != "MyHomeRamen.Persistance"
                        && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsApi, forbiddenPersistence, "Payments API type '{0}' should not depend on persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
