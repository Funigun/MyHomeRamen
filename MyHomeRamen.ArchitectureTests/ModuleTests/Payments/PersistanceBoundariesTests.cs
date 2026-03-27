using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Payments;

public sealed class PersistanceBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void PaymentsPersistance_ShouldDepend_OnlyOn_PaymentsDomain()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Payments", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, otherDomains, "Payments persistence type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsPersistance_ShouldNot_DependOn_MenuPersistance()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, menuPersistence, "Payments persistence type '{0}' should not depend on Menu persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsPersistance_ShouldNot_DependOn_OrdersPersistance()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> ordersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, ordersPersistence, "Payments persistence type '{0}' should not depend on Orders persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsPersistance_ShouldNot_DependOn_ReservationsPersistance()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, reservationsPersistence, "Payments persistence type '{0}' should not depend on Reservations persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsPersistance_ShouldNot_DependOn_ShoppingCartPersistance()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> shoppingCartPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, shoppingCartPersistence, "Payments persistence type '{0}' should not depend on ShoppingCart persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsPersistance_ShouldNot_DependOn_UsersPersistance()
    {
        // Arrange
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Users");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsPersistence, usersPersistence, "Payments persistence type '{0}' should not depend on Users persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
