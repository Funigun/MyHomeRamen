using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class ReservationsModuleBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = reservationsDomain.SelectMany(reservationType =>
            shoppingCartDomain.Select(shoppingCartType =>
                Types().That()
                    .ResideInNamespace(reservationType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(shoppingCartType)
                    .As($"Reservations type '{reservationType}' should not depend on ShoppingCart type '{shoppingCartType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = reservationsDomain.SelectMany(reservationType =>
            menuDomain.Select(menuType =>
                Types().That()
                    .ResideInNamespace(reservationType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(menuType)
                    .As($"Reservations type '{reservationType}' should not depend on Menu type '{menuType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = reservationsDomain.SelectMany(reservationType =>
            ordersDomain.Select(orderType =>
                Types().That()
                    .ResideInNamespace(reservationType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(orderType)
                    .As($"Reservations type '{reservationType}' should not depend on Orders type '{orderType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = reservationsDomain.SelectMany(reservationType =>
            paymentsDomain.Select(paymentType =>
                Types().That()
                    .ResideInNamespace(reservationType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(paymentType)
                    .As($"Reservations type '{reservationType}' should not depend on Payments type '{paymentType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void ReservationsModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");
        IEnumerable<string> usersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Users");

        IEnumerable<IArchRule> rules = reservationsDomain.SelectMany(reservationType =>
            usersDomain.Select(userType =>
                Types().That()
                    .ResideInNamespace(reservationType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(userType)
                    .As($"Reservations type '{reservationType}' should not depend on Users type '{userType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }
}
