using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

public class DataSeeder
{
    internal readonly string SeededUserKeycloakId = "test-keycloak-user-id";

    internal readonly string FullAddressesUserKeycloakId = "test-full-addresses-user-id";

    internal readonly string AnotherUserKeycloakId = "test-another-user-id";

    internal Guid SeededAddressId { get; private set; }

    internal Guid SeededSecondAddressId { get; private set; }

    internal Guid AnotherUserAddressId { get; private set; }

    internal async Task SeedIdentityModule(IIdentityDbContext dbContext)
    {
        IReadOnlyCollection<Permission> permissions = await dbContext.Permission.Query().All(TestContext.Current.CancellationToken);
        PermissionId[] profilePermissionIds =
        [
            permissions.Single(permission => permission.Name == PermissionConstants.CanViewUserProfile).Id,
            permissions.Single(permission => permission.Name == PermissionConstants.CanUpdateUserProfile).Id,
            permissions.Single(permission => permission.Name == PermissionConstants.CanDeleteUserProfile).Id
        ];

        Role role = Role.Create("customer", "Customer role for testing purposes", profilePermissionIds);

        User user = User.Create(
            keycloakUserId: SeededUserKeycloakId,
            userName: "testcustomer",
            firstName: "Test",
            lastName: "Customer",
            email: "testcustomer@example.com",
            phoneNumber: "123456789",
            role: role);

        Address seededAddress = Address.Create("Seeded Street", "1A", string.Empty, "Warsaw", "00-001", isDefault: true);
        SeededAddressId = seededAddress.Id;
        user.AddAddress(seededAddress);

        Address seededSecondAddress = Address.Create("Second Street", "2B", string.Empty, "Warsaw", "00-002", isDefault: false);
        SeededSecondAddressId = seededSecondAddress.Id;
        user.AddAddress(seededSecondAddress);

        dbContext.User.Add(user);

        User anotherUser = User.Create(
            keycloakUserId: AnotherUserKeycloakId,
            userName: "testanotherusr",
            firstName: "Another",
            lastName: "User",
            email: "anotheruser@example.com",
            phoneNumber: "111222333",
            role: role);

        Address anotherUserAddress = Address.Create("Another Street", "2B", string.Empty, "Krakow", "31-001", isDefault: true);
        AnotherUserAddressId = anotherUserAddress.Id;
        anotherUser.AddAddress(anotherUserAddress);

        dbContext.User.Add(anotherUser);

        User fullAddressUser = User.Create(
            keycloakUserId: FullAddressesUserKeycloakId,
            userName: "testfulladdresses",
            firstName: "Full",
            lastName: "Addresses",
            email: "fulladdresses@example.com",
            phoneNumber: "987654321",
            role: role);

        for (int i = 0; i < 5; i++)
        {
            Address address = Address.Create(
                $"Street {i + 1}",
                $"Building{i + 1}",
                string.Empty,
                "Warsaw",
                "00-001",
                isDefault: i == 0);

            fullAddressUser.AddAddress(address);
        }

        dbContext.User.Add(fullAddressUser);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
