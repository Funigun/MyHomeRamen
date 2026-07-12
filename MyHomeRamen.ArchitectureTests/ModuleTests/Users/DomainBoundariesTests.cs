using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ModuleTests.Users;

public sealed class DomainBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void UserModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");
        IEnumerable<string> shoppingCartDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersDomain, shoppingCartDomain, "Users type '{0}' should not depend on ShoppingCart type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UserModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");
        IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersDomain, menuDomain, "Users type '{0}' should not depend on Menu type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UserModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");
        IEnumerable<string> ordersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersDomain, ordersDomain, "Users type '{0}' should not depend on Orders type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UserModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");
        IEnumerable<string> paymentsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersDomain, paymentsDomain, "Users type '{0}' should not depend on Payments type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void UserModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> usersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Identity");
        IEnumerable<string> reservationsDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(usersDomain, reservationsDomain, "Users type '{0}' should not depend on Reservations type '{1}'");

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }
}
