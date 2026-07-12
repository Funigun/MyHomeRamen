using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Reservations;

public sealed class PersistanceBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void ReservationsPersistance_ShouldDepend_OnlyOn_ReservationsDomain()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Reservations", StringComparison.Ordinal) &&
                             !name.StartsWith("MyHomeRamen.Domain.Abstractions", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, otherDomains, "Reservations persistence type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsPersistance_ShouldNot_DependOn_MenuPersistance()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, menuPersistence, "Reservations persistence type '{0}' should not depend on Menu persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsPersistance_ShouldNot_DependOn_OrdersPersistance()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> ordersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, ordersPersistence, "Reservations persistence type '{0}' should not depend on Orders persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsPersistance_ShouldNot_DependOn_PaymentsPersistance()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, paymentsPersistence, "Reservations persistence type '{0}' should not depend on Payments persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsPersistance_ShouldNot_DependOn_ShoppingCartPersistance()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> shoppingCartPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, shoppingCartPersistence, "Reservations persistence type '{0}' should not depend on ShoppingCart persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsPersistance_ShouldNot_DependOn_UsersPersistance()
    {
        // Arrange
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsPersistence, usersPersistence, "Reservations persistence type '{0}' should not depend on Users persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
