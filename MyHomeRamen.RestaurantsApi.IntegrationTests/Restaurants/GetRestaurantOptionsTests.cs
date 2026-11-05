using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantOptions;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.RestaurantsApi.IntegrationTests.Common;

namespace MyHomeRamen.RestaurantsApi.IntegrationTests.Restaurants;

public sealed class GetRestaurantOptionsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task GetRestaurantOptions_ShouldReturnActiveRestaurantAddress_ForAnonymousRequest()
    {
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage("/api/restaurants/options");

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);
        await response.AssertStatusCode(HttpStatusCode.OK);
        GetRestaurantOptionsResponse? result = await response.Content.ReadFromJsonAsync<GetRestaurantOptionsResponse>(
            TestContext.Current.CancellationToken);

        Assert.NotNull(result);
        Assert.Contains(
            result.Restaurants,
            restaurant => restaurant.Id == apiFactory.RestaurantId
                && restaurant.Name == "Integration Restaurant"
                && restaurant.Street == "Main Street 1"
                && restaurant.City == "Warsaw"
                && restaurant.ZipCode == "00-001");
    }
}
