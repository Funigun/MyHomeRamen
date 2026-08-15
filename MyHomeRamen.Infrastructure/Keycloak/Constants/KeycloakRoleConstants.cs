using IdentityDomain = MyHomeRamen.Domain.Identity.Roles.RoleConstants;
using MenuDomain = MyHomeRamen.Domain.Menu.Users.RoleConstants;

namespace MyHomeRamen.Infrastructure.Keycloak.Constants;

public static class KeycloakRoleConstants
{
    internal static IEnumerable<string> AllRoles =>
    [
        IdentityDomain.Employee, IdentityDomain.Customer, IdentityDomain.Admin,
        MenuDomain.Customer, MenuDomain.Employee, MenuDomain.Admin
    ];

    internal static Dictionary<string, IEnumerable<string>> RoleMappings => new()
    {
        [IdentityDomain.Employee] = [IdentityDomain.Employee,
                                    MenuDomain.Employee],

        [IdentityDomain.Customer] = [IdentityDomain.Customer,
                                    MenuDomain.Customer],

        [IdentityDomain.Admin] = [IdentityDomain.Admin,
                                 MenuDomain.Admin],
    };
}
