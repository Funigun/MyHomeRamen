using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Menu;

public sealed class PersistanceBoundariesTests(ArchitectureBuilder architectureBuilder, ITestOutputHelper testOutputHelper) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void MenuPersistance_ShouldDepend_OnlyOn_MenuDomain()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain")
            .Where(name => !name.StartsWith("MyHomeRamen.Domain.Menu", StringComparison.Ordinal) &&
                             !name.StartsWith("MyHomeRamen.Domain.Abstractions", StringComparison.Ordinal));

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, otherDomains, "Menu persistence type '{0}' should not depend on domain type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            testOutputHelper.WriteLine(rule.Description);
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuPersistance_ShouldNot_DependOn_OrdersPersistance()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> ordersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, ordersPersistence, "Menu persistence type '{0}' should not depend on Orders persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuPersistance_ShouldNot_DependOn_PaymentsPersistance()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> paymentsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, paymentsPersistence, "Menu persistence type '{0}' should not depend on Payments persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuPersistance_ShouldNot_DependOn_ReservationsPersistance()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> reservationsPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, reservationsPersistence, "Menu persistence type '{0}' should not depend on Reservations persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuPersistance_ShouldNot_DependOn_ShoppingCartPersistance()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> shoppingCartPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, shoppingCartPersistence, "Menu persistence type '{0}' should not depend on ShoppingCart persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuPersistance_ShouldNot_DependOn_IdentityPersistance()
    {
        // Arrange
        IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Menu");
        IEnumerable<string> usersPersistence = ArchitectureBuilder.PersistanceAssembly.TypesInNamespace("MyHomeRamen.Persistance.Identity");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuPersistence, usersPersistence, "Menu persistence type '{0}' should not depend on Identity persistence type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void CommonPersistance_ShouldNot_DependOn_ApiLayer()
    {
        // Arrange
        IEnumerable<string> commonPersistence = ArchitectureBuilder.PersistanceAssembly
            .TypesInNamespace("MyHomeRamen.Persistance.Common");

        IEnumerable<string> apiTypes = ArchitectureBuilder.ApiFeaturesAssembly
            .TypesInNamespace("MyHomeRamen.Api");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            commonPersistence,
            apiTypes,
            "Common persistence type '{0}' should not depend on API type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            testOutputHelper.WriteLine(rule.Description);
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
