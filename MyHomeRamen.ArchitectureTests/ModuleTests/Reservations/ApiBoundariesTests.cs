using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Reservations;

public sealed class ApiBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void ReservationsApi_ShouldNot_DependOn_MenuApi()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> menuApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, menuApi, "Reservations API type '{0}' should not depend on Menu API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldNot_DependOn_OrdersApi()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> ordersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, ordersApi, "Reservations API type '{0}' should not depend on Orders API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldNot_DependOn_PaymentsApi()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> paymentsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, paymentsApi, "Reservations API type '{0}' should not depend on Payments API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldNot_DependOn_ShoppingCartApi()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> shoppingCartApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, shoppingCartApi, "Reservations API type '{0}' should not depend on ShoppingCart API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldNot_DependOn_UsersApi()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> usersApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Identity");
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, usersApi, "Reservations API type '{0}' should not depend on Users API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldDepend_OnlyOn_ReservationsDomain()
    {
        // Arrange
        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Reservations", StringComparison.Ordinal) && !name.StartsWith("MyHomeRamen.Domain.Abstractions.AuditableEntity", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, otherDomains, "Reservations API type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsApi_ShouldDepend_OnlyOn_ReservationsPersistance()
    {
        // Arrange
        IEnumerable<string> allowedPersistanceNamespaces =
        [
            "MyHomeRamen.Persistance.Reservations",
            "MyHomeRamen.Persistance.Common",
        ];

        IEnumerable<string> reservationsApi = ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace("MyHomeRamen.Features.Reservations");
        IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance")
            .Where(name => name != "MyHomeRamen.Persistance"
                        && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsApi, forbiddenPersistence, "Reservations API type '{0}' should not depend on persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
