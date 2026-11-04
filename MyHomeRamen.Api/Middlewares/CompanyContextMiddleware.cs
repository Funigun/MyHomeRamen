using MyHomeRamen.Features.Common.Configurations;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Restaurants.ExternalApi;

namespace MyHomeRamen.Api.Middlewares;

public sealed class CompanyContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICompanyContext companyContext, IRestaurantService restaurantService)
    {
        bool isTenantScopedRequest = context.Request.Path.StartsWithSegments("/api/menu", StringComparison.OrdinalIgnoreCase)
                                  || context.Request.Path.StartsWithSegments("/api/payments", StringComparison.OrdinalIgnoreCase)
                                  || context.Request.Path.StartsWithSegments("/api/shopping-cart", StringComparison.OrdinalIgnoreCase);
        bool isMenuCardOptionsRequest = context.Request.Path.Equals(
            "/api/menu/cards/options",
            StringComparison.OrdinalIgnoreCase);
        bool isCurrentBasketDiscoveryRequest =
            context.Request.Path.Equals("/api/shopping-cart/basket/details", StringComparison.OrdinalIgnoreCase)
            || context.Request.Path.Equals("/api/shopping-cart/basket/summary", StringComparison.OrdinalIgnoreCase);

        if (!context.Request.Headers.TryGetValue(HeaderConstants.RestaurantId, out Microsoft.Extensions.Primitives.StringValues restaurantIdHeader))
        {
            if (isTenantScopedRequest && !isMenuCardOptionsRequest && !isCurrentBasketDiscoveryRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = $"{HeaderConstants.RestaurantId} header is required." }, CancellationToken.None);
                return;
            }

            await next(context);
            return;
        }

        if (restaurantIdHeader.Count != 1
            || !Guid.TryParse(restaurantIdHeader[0], out Guid restaurantId)
            || restaurantId == Guid.Empty)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = $"{HeaderConstants.RestaurantId} header must contain a valid restaurant identifier." }, CancellationToken.None);
            return;
        }

        RestaurantInfo? restaurant = await restaurantService.GetRestaurantAsync(restaurantId, context.RequestAborted);

        if (restaurant is null || !restaurant.IsActive)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        companyContext.SetRestaurant(restaurant.Id, restaurant.CompanyId);
        await next(context);
    }
}
