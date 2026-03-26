using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class MenuModuleBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void MenuModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = menuDomain.SelectMany(menuType =>
            shoppingCartDomain.Select(shoppingCartType =>
                Types().That()
                    .ResideInNamespace(menuType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(shoppingCartType)
                    .As($"Menu type '{menuType}' should not depend on ShoppingCart type '{shoppingCartType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = menuDomain.SelectMany(menuType =>
            ordersDomain.Select(orderType =>
                Types().That()
                    .ResideInNamespace(menuType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(orderType)
                    .As($"Menu type '{menuType}' should not depend on Orders type '{orderType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = menuDomain.SelectMany(menuType =>
            paymentsDomain.Select(paymentType =>
                Types().That()
                    .ResideInNamespace(menuType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(paymentType)
                    .As($"Menu type '{menuType}' should not depend on Payments type '{paymentType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = menuDomain.SelectMany(menuType =>
            reservationsDomain.Select(reservationType =>
                Types().That()
                    .ResideInNamespace(menuType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(reservationType)
                    .As($"Menu type '{menuType}' should not depend on Reservations type '{reservationType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void MenuModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
        IEnumerable<string> usersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Users");

        IEnumerable<IArchRule> rules = menuDomain.SelectMany(menuType =>
            usersDomain.Select(userType =>
                Types().That()
                    .ResideInNamespace(menuType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(userType)
                    .As($"Menu type '{menuType}' should not depend on Users type '{userType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }
}
