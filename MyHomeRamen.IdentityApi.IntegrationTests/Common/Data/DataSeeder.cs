using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;

namespace MyHomeRamen.IdentityApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
    internal static readonly string SeededUserKeycloakId = "test-keycloak-user-id";

    internal static readonly Guid SeededRestaurantId = Guid.Parse("fac13f05-5688-4169-9f89-927ae708dd35");

    internal static readonly string FullAddressesUserKeycloakId = "test-full-addresses-user-id";

    internal static readonly string AnotherUserKeycloakId = "test-another-user-id";

    internal static Guid SeededAddressId { get; private set; }

    internal static Guid SeededSecondAddressId { get; private set; }

    internal static Guid AnotherUserAddressId { get; private set; }

    internal static async Task SeedIdentityModule(IIdentityDbContext dbContext)
    {
        User user = User.Create(
            keycloakUserId: SeededUserKeycloakId,
            userName: "testcustomer",
            firstName: "Test",
            lastName: "Customer",
            email: "testcustomer@example.com",
            phoneNumber: "123456789",
            role: "customer");

        dbContext.User.Add(user);

        User anotherUser = User.Create(
            keycloakUserId: AnotherUserKeycloakId,
            userName: "testanotherusr",
            firstName: "Another",
            lastName: "User",
            email: "anotheruser@example.com",
            phoneNumber: "111222333",
            role: "customer");

        dbContext.User.Add(anotherUser);

        User fullAddressUser = User.Create(
            keycloakUserId: FullAddressesUserKeycloakId,
            userName: "testfulladdresses",
            firstName: "Full",
            lastName: "Addresses",
            email: "fulladdresses@example.com",
            phoneNumber: "987654321",
            role: "customer");

        dbContext.User.Add(fullAddressUser);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        User? loadedUser = await dbContext.User.Specification().ByExternalId(SeededUserKeycloakId, TestContext.Current.CancellationToken);

        if (loadedUser is not null)
        {
            Address seededAddress = Address.Create(Guid.NewGuid(), "Seeded Street", "1A", string.Empty, "Warsaw", "00-001", isDefault: true);
            SeededAddressId = seededAddress.Id;
            loadedUser.AddAddress(seededAddress);

            Address seededSecondAddress = Address.Create(Guid.NewGuid(), "Second Street", "2B", string.Empty, "Warsaw", "00-002", isDefault: false);
            SeededSecondAddressId = seededSecondAddress.Id;
            loadedUser.AddAddress(seededSecondAddress);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        User? loadedAnotherUser = await dbContext.User.Specification().ByExternalId(AnotherUserKeycloakId, TestContext.Current.CancellationToken);

        if (loadedAnotherUser is not null)
        {
            Address anotherUserAddress = Address.Create(Guid.NewGuid(), "Another Street", "2B", string.Empty, "Krakow", "31-001", isDefault: true);
            AnotherUserAddressId = anotherUserAddress.Id;
            loadedAnotherUser.AddAddress(anotherUserAddress);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        User? loadedFullUser = await dbContext.User.Specification().ByExternalId(FullAddressesUserKeycloakId, TestContext.Current.CancellationToken);

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
            }

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }
}
