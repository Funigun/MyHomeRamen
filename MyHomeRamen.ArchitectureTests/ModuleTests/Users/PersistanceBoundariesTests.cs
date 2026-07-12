using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Users;

public sealed class PersistanceBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void UsersPersistance_ShouldDepend_OnlyOn_UsersDomain()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Identity", StringComparison.Ordinal) &&
                             !name.StartsWith("MyHomeRamen.Domain.Abstractions", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, otherDomains, "Users persistence type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersPersistance_ShouldNot_DependOn_MenuPersistance()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, menuPersistence, "Users persistence type '{0}' should not depend on Menu persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersPersistance_ShouldNot_DependOn_OrdersPersistance()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> ordersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, ordersPersistence, "Users persistence type '{0}' should not depend on Orders persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersPersistance_ShouldNot_DependOn_PaymentsPersistance()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, paymentsPersistence, "Users persistence type '{0}' should not depend on Payments persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersPersistance_ShouldNot_DependOn_ReservationsPersistance()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, reservationsPersistence, "Users persistence type '{0}' should not depend on Reservations persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UsersPersistance_ShouldNot_DependOn_ShoppingCartPersistance()
    {
        // Arrange
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");
        IEnumerable<string> shoppingCartPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersPersistence, shoppingCartPersistence, "Users persistence type '{0}' should not depend on ShoppingCart persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
