using System.Reflection;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.UnitTests.IdentityModule.Permissions;

public sealed class PermissionConstantsTests
{
    public static TheoryData<Type, Type> PermissionConstants =>
    [
        new(typeof(IdentityPermissionConstants), typeof(MyHomeRamen.Domain.Identity.Permissions.PermissionConstants)),
        new(typeof(MenuPermissionConstants), typeof(MyHomeRamen.Domain.Menu.Users.PermissionConstants)),
        new(typeof(ShoppingCartPermissionConstants), typeof(MyHomeRamen.Domain.ShoppingCart.Users.PermissionConstants)),
        new(typeof(OrdersPermissionConstants), typeof(MyHomeRamen.Domain.Orders.Users.PermissionConstants)),
        new(typeof(ReservationsPermissionConstants), typeof(MyHomeRamen.Domain.Reservations.Users.PermissionConstants)),
        new(typeof(PaymentsPermissionConstants), typeof(MyHomeRamen.Domain.Payments.Users.PermissionConstants)),
        new(typeof(RestaurantsPermissionConstants), typeof(MyHomeRamen.Domain.Restaurants.Users.PermissionConstants))
    ];

    [Theory]
    [MemberData(nameof(PermissionConstants))]
    public void PermissionConstants_ShouldMatchDomainConstants_WhenIdentityModuleSeedsPermissions(Type identityConstantsType, Type domainConstantsType)
    {
        FieldInfo[] identityConstants = GetConstants(identityConstantsType);
        FieldInfo[] domainConstants = GetConstants(domainConstantsType);

        Assert.Equal(domainConstants.Length, identityConstants.Length);

        foreach (FieldInfo identityConstant in identityConstants)
        {
            FieldInfo? domainConstant = domainConstants.SingleOrDefault(constant => constant.Name == identityConstant.Name);

            Assert.NotNull(domainConstant);
            Assert.Equal(identityConstant.GetRawConstantValue(), domainConstant!.GetRawConstantValue());
        }
    }

    private static FieldInfo[] GetConstants(Type constantsType) =>
        constantsType.GetFields(BindingFlags.Public | BindingFlags.Static)
                     .Where(field => field.IsLiteral && !field.IsInitOnly)
                     .ToArray();
}
