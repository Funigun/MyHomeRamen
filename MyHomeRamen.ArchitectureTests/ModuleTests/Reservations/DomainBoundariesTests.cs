using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Reservations;

public sealed class DomainBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> shoppingCartDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsDomain, shoppingCartDomain, "Reservations type '{0}' should not depend on ShoppingCart type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsDomain, menuDomain, "Reservations type '{0}' should not depend on Menu type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> ordersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsDomain, ordersDomain, "Reservations type '{0}' should not depend on Orders type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsDomain, paymentsDomain, "Reservations type '{0}' should not depend on Payments type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(reservationsDomain, usersDomain, "Reservations type '{0}' should not depend on Users type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
