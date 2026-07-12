    using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Menu;

public sealed class DomainBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void MenuModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> shoppingCartDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuDomain, shoppingCartDomain, "Menu type '{0}' should not depend on ShoppingCart type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> ordersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuDomain, ordersDomain, "Menu type '{0}' should not depend on Orders type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuDomain, paymentsDomain, "Menu type '{0}' should not depend on Payments type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuDomain, reservationsDomain, "Menu type '{0}' should not depend on Reservations type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> identityDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(menuDomain, identityDomain, "Menu type '{0}' should not depend on Identityq     type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
