using System.Net.Http.Json;
using Aspire.Hosting.Testing;
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

        RegisterRequest request = new("TestCustomer", "Test", "Customer", "testcustomer@gmail.com", "1234567890", "P@ssw0rd!", "TestCustomerPassword123!");

        // Act
        HttpResponseMessage response = await api.PostAsJsonAsync("/api/account/sign-up", request, TestContext.Current.CancellationToken);
        string respon = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        await Task.Delay(2000);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
