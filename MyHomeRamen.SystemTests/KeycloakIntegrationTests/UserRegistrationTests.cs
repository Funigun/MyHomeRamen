using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.SystemTests.Config;

namespace MyHomeRamen.SystemTests.KeycloakIntegrationTests;

public sealed class UserRegistrationTests(AppConfigurationFixture appConfigurationFixture)
{
    [Fact]
    public async Task UserRegistration_ShouldCreateUserInAllModules()
    {
        // Arrange
        using HttpClient api = appConfigurationFixture.Application.CreateHttpClient(AppConfigurationFixture.IdentityApiResourceName);

        string user = Guid.NewGuid().ToString();
        RegisterRequest request = new(user, user.Substring(0, 5), user.Substring(6, 5), $"{user}@gmail.com", "1234567890", "TestCustomerPassword123!", "TestCustomerPassword123!");

        // Act
        await api.PostAsJsonAsync("/api/account/sign-up", request, TestContext.Current.CancellationToken);

        // Allow time for async message processing
        await Task.Delay(5000, TestContext.Current.CancellationToken);

        // Verify user exists in all module databases
        string userId = await GetCreatedUserId(user);
        await AssertUserExistsInModule("menu", userId);
        await AssertUserExistsInModule("orders", userId);
        await AssertUserExistsInModule("payments", userId);
        await AssertUserExistsInModule("reservations", userId);
        await AssertUserExistsInModule("basket", userId);
    }

    private static async Task<string> GetCreatedUserId(string user)
    {
        using SqlConnection connection = new(AppConfigurationFixture.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        using SqlCommand command = new("SELECT Id FROM [MyHomeRamenTest].[identity].[Users] WHERE [UserName] = @user", connection);
        command.Parameters.AddWithValue("@user", user);

        object? result = await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);

        return result?.ToString() ?? throw new InvalidOperationException($"User '{user}' not found.");
    }

    private static async Task AssertUserExistsInModule(string schema, string user)
    {
        using SqlConnection connection = new(AppConfigurationFixture.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        using SqlCommand command = new($"SELECT COUNT(*) FROM {schema}.Users WHERE ID = @user", connection);
        command.Parameters.AddWithValue("@user", user.ToUpper());

        int count = (int)await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);
        Assert.True(count == 1, $"User with email '{user}' not found in {schema} module");
    }
}
