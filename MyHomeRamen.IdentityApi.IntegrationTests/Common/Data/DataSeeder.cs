using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Domain.Users.Database;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
    internal static readonly string SeededUserKeycloakId = "test-keycloak-user-id";

    internal static readonly Guid SeededRestaurantId = Guid.Parse("fac13f05-5688-4169-9f89-927ae708dd35");

    internal static readonly string FullAddressesUserKeycloakId = "test-full-addresses-user-id";

    internal static async Task SeedIdentityModule(IUsersDbContext dbContext)
    {
        User user = User.Create(
            keycloakUserId: SeededUserKeycloakId,
            userName: "testcustomer",
            firstName: "Test",
            lastName: "Customer",
            email: "testcustomer@example.com",
            phoneNumber: "123456789",
            role: "customer");

        dbContext.Users.Add(user);

        User fullAddressUser = User.Create(
            keycloakUserId: FullAddressesUserKeycloakId,
            userName: "testfulladdresses",
            firstName: "Full",
            lastName: "Addresses",
            email: "fulladdresses@example.com",
            phoneNumber: "987654321",
            role: "customer");

        dbContext.Users.Add(fullAddressUser);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        User? loadedFullUser = await dbContext.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.KeycloakUserId == FullAddressesUserKeycloakId, TestContext.Current.CancellationToken);

        if (loadedFullUser is not null)
        {
            for (int i = 0; i < 5; i++)
            {
                Address address = Address.Create(
                    Guid.NewGuid(),
                    $"Street {i + 1}",
                    $"Building{i + 1}",
                    string.Empty,
                    "Warsaw",
                    "00-001",
                    isDefault: i == 0);

                loadedFullUser.AddAddress(address);
                dbContext.Addresses.Add(address);
            }

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }
}
