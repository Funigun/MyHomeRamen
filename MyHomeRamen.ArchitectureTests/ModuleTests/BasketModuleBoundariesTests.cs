using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace MyHomeRamen.ArchitectureTests.ModuleTests;

public sealed class BasketModuleBoundariesTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void BasketModule_ShouldNot_DependOn_MenuModule()
    {
        // Arrange
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

        IEnumerable<IArchRule> rules = shoppingCartDomain.SelectMany(shoppingCartType =>
            menuDomain.Select(menuType =>
                Types().That()
                    .ResideInNamespace(shoppingCartType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(menuType)
                    .As($"ShoppingCart type '{shoppingCartType}' should not depend on Menu type '{menuType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void BasketModule_ShouldNot_DependOn_OrdersModule()
    {
        // Arrange
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

        IEnumerable<IArchRule> rules = shoppingCartDomain.SelectMany(shoppingCartType =>
            menuDomain.Select(orderType =>
                Types().That()
                    .ResideInNamespace(shoppingCartType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(orderType)
                    .As($"ShoppingCart type '{shoppingCartType}' should not depend on Order type '{orderType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void BasketModule_ShouldNot_DependOn_ReservationsModule()
    {
        // Arrange
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Reservations");

        IEnumerable<IArchRule> rules = shoppingCartDomain.SelectMany(shoppingCartType =>
            menuDomain.Select(reservationsType =>
                Types().That()
                    .ResideInNamespace(shoppingCartType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(reservationsType)
                    .As($"ShoppingCart type '{shoppingCartType}' should not depend on Reservation type '{reservationsType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void BasketModule_ShouldNot_DependOn_PaymentsModule()
    {
        // Arrange
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Payments");

        IEnumerable<IArchRule> rules = shoppingCartDomain.SelectMany(shoppingCartType =>
            menuDomain.Select(paymentType =>
                Types().That()
                    .ResideInNamespace(shoppingCartType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(paymentType)
                    .As($"ShoppingCart type '{shoppingCartType}' should not depend on Payment type '{paymentType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }

    [Fact]
    public void BasketModule_ShouldNot_DependOn_UsersModule()
    {
        // Arrange
        IEnumerable<string> shoppingCartDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.ShoppingCart");
        IEnumerable<string> menuDomain = DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Users");

        IEnumerable<IArchRule> rules = shoppingCartDomain.SelectMany(shoppingCartType =>
            menuDomain.Select(userType =>
                Types().That()
                    .ResideInNamespace(shoppingCartType)
                    .Should()
                    .NotDependOnAnyTypesThat()
                    .ResideInNamespace(userType)
                    .As($"ShoppingCart type '{shoppingCartType}' should not depend on User type '{userType}'")
            )
        );

        // Act & Assert
        foreach (IArchRule rule in rules)
        {
            rule.Check(Architecture);
        }
    }
}
