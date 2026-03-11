using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;
using MyHomeRamen.SystemTests.Config;

namespace MyHomeRamen.SystemTests.KeycloakIntegrationTests;

public sealed class UserRegistrationTests(AppConfigurationFixture appConfigurationFixture)
{
    private const string Connection = "Server=.;Database=MyHomeRamenTest;Trusted_Connection=True;TrustServerCertificate=True";

    [Fact]
    public async Task UserRegistration_ShouldCreateUserInAllModules()
    {
        // Arrange
        HttpClient api = appConfigurationFixture.Application.CreateHttpClient(AppConfigurationFixture.IdentityApiResourceName);
        string user = Guid.NewGuid().ToString();
        RegisterRequest request = new(user, user.Substring(0, 5), user.Substring(6, 5), $"{user}@gmail.com", "1234567890", "TestCustomerPassword123!", "TestCustomerPassword123!");

        // Act
        HttpResponseMessage response = await api.PostAsJsonAsync("/api/account/sign-up", request, TestContext.Current.CancellationToken);
 
        string respon = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        await Task.Delay(5000); // Allow time for async message processing

        // Assert
        response.EnsureSuccessStatusCode();

        // Verify user exists in all module databases
        IConfiguration config = appConfigurationFixture.Application.Services.GetRequiredService<IConfiguration>();
        string userId = await GetCreatedUserId(Connection, user);
        await AssertUserExistsInModule(Connection, "menu", userId);
        await AssertUserExistsInModule(Connection, "orders", userId);
        await AssertUserExistsInModule(Connection, "payments", userId);
        await AssertUserExistsInModule(Connection, "reservations", userId);
        await AssertUserExistsInModule(Connection, "basket", userId);
    }

    private static async Task<string> GetCreatedUserId(string connectionString, string user)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        using SqlCommand command = new("SELECT Id FROM [MyHomeRamenTest].[identity].[Users] WHERE [UserName] = @user", connection);
        command.Parameters.AddWithValue("@user", user);

        object? result = await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);

        return result?.ToString() ?? throw new InvalidOperationException($"User '{user}' not found.");
    }

    private static async Task AssertUserExistsInModule(string connectionString, string schema, string user)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        using SqlCommand command = new($"SELECT COUNT(*) FROM {schema}.Users WHERE ID = @user", connection);
        command.Parameters.AddWithValue("@user", user.ToUpper());

        int count = (int)await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);
        Assert.True(count == 1, $"User with email '{user}' not found in {schema} module");
    }
}
