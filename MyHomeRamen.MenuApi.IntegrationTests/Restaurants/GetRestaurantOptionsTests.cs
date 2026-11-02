using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetRestaurantOptions;
using MyHomeRamen.IntegrationTests.Extensions;
using MyHomeRamen.MenuApi.IntegrationTests.Common;
using MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

namespace MyHomeRamen.MenuApi.IntegrationTests.Restaurants;

public sealed class GetRestaurantOptionsTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>
{
    [Fact]
    public async Task GetRestaurantOptions_ShouldReturnActiveRestaurantAddress_ForAnonymousRequest()
    {
        using HttpRequestMessage request = HttpClientExtensions.CreateGetMessage("/api/restaurants/options");

        HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
            request,
            TestContext.Current.CancellationToken);
        GetRestaurantOptionsResponse? result = await response.Content.ReadFromJsonAsync<GetRestaurantOptionsResponse>(
            TestContext.Current.CancellationToken);

        await response.AssertStatusCode(HttpStatusCode.OK);
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
