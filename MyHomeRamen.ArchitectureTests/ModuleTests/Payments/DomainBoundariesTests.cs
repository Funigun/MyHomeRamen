using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Payments;

public sealed class DomainBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> shoppingCartDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsDomain, shoppingCartDomain, "Payments type '{0}' should not depend on ShoppingCart type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsDomain, menuDomain, "Payments type '{0}' should not depend on Menu type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> ordersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsDomain, ordersDomain, "Payments type '{0}' should not depend on Orders type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsDomain, reservationsDomain, "Payments type '{0}' should not depend on Reservations type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(paymentsDomain, usersDomain, "Payments type '{0}' should not depend on Users type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
