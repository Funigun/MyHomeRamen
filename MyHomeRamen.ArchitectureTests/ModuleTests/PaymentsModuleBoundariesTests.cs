using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class PaymentsModuleBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = paymentsDomain.SelectMany(paymentType =>
            shoppingCartDomain.Select(shoppingCartType =>
                Types().That()
                    .ResideInNamespace(paymentType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(shoppingCartType)
                    .As($"Payments type '{paymentType}' should not depend on ShoppingCart type '{shoppingCartType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = paymentsDomain.SelectMany(paymentType =>
            menuDomain.Select(menuType =>
                Types().That()
                    .ResideInNamespace(paymentType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(menuType)
                    .As($"Payments type '{paymentType}' should not depend on Menu type '{menuType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = paymentsDomain.SelectMany(paymentType =>
            ordersDomain.Select(orderType =>
                Types().That()
                    .ResideInNamespace(paymentType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(orderType)
                    .As($"Payments type '{paymentType}' should not depend on Orders type '{orderType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = paymentsDomain.SelectMany(paymentType =>
            reservationsDomain.Select(reservationType =>
                Types().That()
                    .ResideInNamespace(paymentType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(reservationType)
                    .As($"Payments type '{paymentType}' should not depend on Reservations type '{reservationType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void PaymentsModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");
        IEnumerable<string> usersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Users");

        IEnumerable<IArchRule> rules = paymentsDomain.SelectMany(paymentType =>
            usersDomain.Select(userType =>
                Types().That()
                    .ResideInNamespace(paymentType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(userType)
                    .As($"Payments type '{paymentType}' should not depend on Users type '{userType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }
}
