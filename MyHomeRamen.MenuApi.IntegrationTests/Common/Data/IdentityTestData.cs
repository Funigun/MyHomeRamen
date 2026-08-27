using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using SharedIdentityTestData = MyHomeRamen.IntegrationTests.Identity.IdentityTestData;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

public sealed class IdentityTestData(SharedIdentityTestData sharedIdentityTestData)
{
    internal (string KeycloakUserId, Guid UserId) AdminUser { get; private set; }

    internal (string KeycloakUserId, Guid UserId) CustomerUser { get; private set; }

    internal (string KeycloakUserId, Guid UserId) EmployeeUser { get; private set; }

    internal (string KeycloakUserId, Guid UserId) GetUser(UserRoles role)
        => role switch
        {
            UserRoles.Customer => CustomerUser,
            UserRoles.Employee => EmployeeUser,
            _ => AdminUser
        };

    internal async Task SeedAsync()
    {
        AdminUser = await sharedIdentityTestData.SeedUser(
            ("Menu Admin", PermissionConstants.AvailablePermissions),
            "menu-admin",
            "Admin");

        CustomerUser = await sharedIdentityTestData.SeedUser(
            ("Customer", []),
            "menu-customer",
            "Customer");

        EmployeeUser = await sharedIdentityTestData.SeedUser(
             ("Employee", []),
             "menu-employee",
             "Employee");
    }
}
