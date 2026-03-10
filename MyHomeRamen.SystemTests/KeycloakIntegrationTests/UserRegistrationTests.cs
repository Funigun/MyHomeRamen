using System.Net.Http.Json;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;
using MyHomeRamen.SystemTests.Config;

namespace MyHomeRamen.SystemTests.KeycloakIntegrationTests;

public sealed class UserRegistrationTests(AppConfigurationFixture appConfigurationFixture)
{
    [Fact]
    public async Task UserRegistration_ShouldCreateUserInAllModules()
    {
        // Arrange
        HttpClient api = appConfigurationFixture.Application.CreateHttpClient(AppConfigurationFixture.IdentityApiResourceName);

        RegisterRequest request = new("TestCustomer", "Test", "Customer", "testcustomer@gmail.com", "1234567890", "TestCustomerPassword123!", "TestCustomerPassword123!");

        // Act
        HttpResponseMessage response = await api.PostAsJsonAsync("/api/account/sign-up", request, TestContext.Current.CancellationToken);
        string respon = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        await Task.Delay(2000); // Allow time for async message processing

        // Assert
        response.EnsureSuccessStatusCode();

        // Verify user exists in all module databases
        IConfiguration config = appConfigurationFixture.Application.Services.GetRequiredService<IConfiguration>();

        await AssertUserExistsInModule(config["RestaurantConfiguration:WorkerConnectionString"]!, "menu", request.Email);
        await AssertUserExistsInModule(config["RestaurantConfiguration:WorkerConnectionString"]!, "orders", request.Email);
        await AssertUserExistsInModule(config["RestaurantConfiguration:WorkerConnectionString"]!, "payments", request.Email);
        await AssertUserExistsInModule(config["RestaurantConfiguration:WorkerConnectionString"]!, "reservations", request.Email);
        await AssertUserExistsInModule(config["RestaurantConfiguration:WorkerConnectionString"]!, "shoppingcart", request.Email);
    }

    private static async Task AssertUserExistsInModule(string connectionString, string schema, string email)
    {
        using SqlConnection connection = new(connectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        using SqlCommand command = new($"SELECT COUNT(*) FROM {schema}.Users WHERE Email = @email", connection);
        command.Parameters.AddWithValue("@email", email);

        int count = (int)await command.ExecuteScalarAsync(TestContext.Current.CancellationToken);
        Assert.True(count == 1, $"User with email '{email}' not found in {schema} module");
    }
}
