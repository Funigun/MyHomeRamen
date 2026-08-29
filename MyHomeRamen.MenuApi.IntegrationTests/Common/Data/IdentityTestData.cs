using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Authentication;
using SharedIdentityTestData = MyHomeRamen.IntegrationTests.Identity.IdentityTestData;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

public sealed class IdentityTestData(SharedIdentityTestData sharedIdentityTestData)
{
    internal (string KeycloakUserId, Guid UserId) CustomerUser { get; private set; }

    internal (string KeycloakUserId, Guid UserId) EmployeeUser { get; private set; }

    internal (string KeycloakUserId, Guid UserId) GetUser(UserRoles role)
        => role switch
        {
            UserRoles.Customer => CustomerUser,
            UserRoles.Employee => EmployeeUser,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Only forbidden roles are supported.")
        };

    internal async Task SeedAsync()
    {
        CustomerUser = await sharedIdentityTestData.SeedUser(
            ("Customer", []),
            "menu-customer",
            "Customer");

        EmployeeUser = await sharedIdentityTestData.SeedUser(
             ("Employee", []),
             "menu-employee",
             "Employee");
    }

    internal Task<(string KeycloakUserId, Guid UserId)> SeedUser(IEnumerable<string> permissions, string userName)
        => sharedIdentityTestData.SeedUser(("Menu Test User", permissions), userName, "Test");

    internal Task DeleteUser(Guid userId)
        => sharedIdentityTestData.DeleteUser(userId);
}
