using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Restaurants.Users;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetEmployeeRestaurantOptions;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.RestaurantsApi.IntegrationTests.Common;

namespace MyHomeRamen.RestaurantsApi.IntegrationTests.Restaurants;

public sealed class GetEmployeeRestaurantOptionsTests(WebApiFactory apiFactory)
    : IClassFixture<WebApiFactory>, IAsyncLifetime
{
    private const string Endpoint = "/api/restaurants/employee-options";
    private Guid _secondRestaurantId;
    private Guid _inactiveRestaurantId;
    private string _secondRestaurantName = string.Empty;
    private (string KeycloakId, Guid UserId) _employee;

    public async ValueTask InitializeAsync()
    {
        _secondRestaurantName = $"Employee Restaurant Two {Guid.CreateVersion7():N}";
        _secondRestaurantId = await apiFactory.SeedRestaurantAsync(_secondRestaurantName);
        _inactiveRestaurantId = await apiFactory.SeedRestaurantAsync(
            $"Inactive Employee Restaurant {Guid.CreateVersion7():N}",
            false);
        _employee = await apiFactory.IdentityTestData.SeedRestaurantScopedUser(
            [PermissionConstants.RestaurantDetailsEdit, PermissionConstants.RestaurantBankDetailsEdit],
            [apiFactory.RestaurantId, _secondRestaurantId, _inactiveRestaurantId],
            $"employee-options-{Guid.CreateVersion7():N}");
    }

    public async ValueTask DisposeAsync()
    {
        await apiFactory.IdentityTestData.DeleteUser(_employee.UserId);
    }

    [Fact]
    public async Task GetEmployeeRestaurantOptions_ShouldReturnActiveAssignedRestaurants_WithDistinctIdsAndLocationFields()
    {
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);
        request.AddAuthorizationHeader(_employee);

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetEmployeeRestaurantOptionsResponse? result = await response.Content.ReadFromJsonAsync<GetEmployeeRestaurantOptionsResponse>(
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.Restaurants.Count());
        Assert.Contains(result.Restaurants, restaurant => restaurant.Id == apiFactory.RestaurantId);
        EmployeeRestaurantOptionDto secondRestaurant = Assert.Single(
            result.Restaurants.Where(restaurant => restaurant.Id == _secondRestaurantId));
        Assert.Equal(_secondRestaurantName, secondRestaurant.Name);
        Assert.Equal($"{_secondRestaurantName} Street", secondRestaurant.Street);
        Assert.Equal("Warsaw", secondRestaurant.City);
        Assert.Equal("00-001", secondRestaurant.ZipCode);
        Assert.Equal(52.2297, secondRestaurant.Latitude);
        Assert.Equal(21.0122, secondRestaurant.Longitude);
        Assert.DoesNotContain(result.Restaurants, restaurant => restaurant.Id == _inactiveRestaurantId);
    }

    [Fact]
    public async Task GetEmployeeRestaurantOptions_ShouldAllowNonManagementResourcePermission()
    {
        (string KeycloakId, Guid UserId) employee = await apiFactory.IdentityTestData.SeedRestaurantScopedUser(
            [PermissionConstants.RestaurantDetailsEdit],
            [apiFactory.RestaurantId],
            $"employee-view-only-{Guid.CreateVersion7():N}");

        try
        {
            using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);
            request.AddAuthorizationHeader(employee);

            HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);
            await response.AssertStatusCode(HttpStatusCode.OK);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(employee.UserId);
        }
    }

    [Fact]
    public async Task GetEmployeeRestaurantOptions_ShouldReturnForbidden_ForCompanyScopedGrantOnly()
    {
        (string KeycloakId, Guid UserId) employee = await apiFactory.IdentityTestData.SeedUser(
            ("Company employee", [PermissionConstants.RestaurantDetailsEdit]),
            $"employee-company-only-{Guid.CreateVersion7():N}",
            "Test");

        try
        {
            using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);
            request.AddAuthorizationHeader(employee);

            HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);
            await response.AssertStatusCode(HttpStatusCode.Forbidden);
        }
        finally
        {
            await apiFactory.IdentityTestData.DeleteUser(employee.UserId);
        }
    }

    [Fact]
    public async Task GetEmployeeRestaurantOptions_ShouldReturnUnauthorized_ForAnonymousUser()
    {
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage(Endpoint);

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(request, TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.Unauthorized);
    }
}
