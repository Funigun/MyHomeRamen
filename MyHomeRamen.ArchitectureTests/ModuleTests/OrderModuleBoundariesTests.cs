using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class OrderModuleBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void OrderModule_ShouldNot_DependOn_ShoppingCartModule()
    {
        // Arrange
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");

        IEnumerable<IArchRule> rules = ordersDomain.SelectMany(orderType =>
            shoppingCartDomain.Select(shoppingCartType =>
                Types().That()
                    .ResideInNamespace(orderType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(shoppingCartType)
                    .As($"Orders type '{orderType}' should not depend on ShoppingCart type '{shoppingCartType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void OrderModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = ordersDomain.SelectMany(orderType =>
            menuDomain.Select(menuType =>
                Types().That()
                    .ResideInNamespace(orderType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(menuType)
                    .As($"Orders type '{orderType}' should not depend on Menu type '{menuType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void OrderModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");
        IEnumerable<string> paymentsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = ordersDomain.SelectMany(orderType =>
            paymentsDomain.Select(paymentType =>
                Types().That()
                    .ResideInNamespace(orderType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(paymentType)
                    .As($"Orders type '{orderType}' should not depend on Payments type '{paymentType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void OrderModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");
        IEnumerable<string> reservationsDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = ordersDomain.SelectMany(orderType =>
            reservationsDomain.Select(reservationType =>
                Types().That()
                    .ResideInNamespace(orderType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(reservationType)
                    .As($"Orders type '{orderType}' should not depend on Reservations type '{reservationType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void OrderModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> ordersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");
        IEnumerable<string> usersDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Users");

        IEnumerable<IArchRule> rules = ordersDomain.SelectMany(orderType =>
            usersDomain.Select(userType =>
                Types().That()
                    .ResideInNamespace(orderType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(userType)
                    .As($"Orders type '{orderType}' should not depend on Users type '{userType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }
}
