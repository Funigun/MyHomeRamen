using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MyHomeRamen.Blazor.Features.Restaurants.Common.Services;

public sealed class RestaurantContextService(ProtectedSessionStorage sessionStorage)
{
    private const string EmployeeContextKey = "restaurant-context:employee";
    private const string CustomerContextKey = "restaurant-context:customer";

    public async Task<Guid?> GetEmployeeRestaurantIdAsync(Guid authenticatedUserId)
    {
        EnsureValidId(authenticatedUserId, nameof(authenticatedUserId));
        ProtectedBrowserStorageResult<EmployeeContext> result =
            await sessionStorage.GetAsync<EmployeeContext>(EmployeeContextKey);
        if (!result.Success)
        {
            return null;
        }

        EmployeeContext? context = result.Value;
        if (context is null || context.UserId != authenticatedUserId || context.RestaurantId == Guid.Empty)
        {
            await ClearEmployeeRestaurantAsync();
            return null;
        }

        return context.RestaurantId;
    }

    public async Task SetEmployeeRestaurantIdAsync(Guid authenticatedUserId, Guid restaurantId)
    {
        EnsureValidId(authenticatedUserId, nameof(authenticatedUserId));
        EnsureValidId(restaurantId, nameof(restaurantId));
        await sessionStorage.SetAsync(EmployeeContextKey, new EmployeeContext(authenticatedUserId, restaurantId));
    }

    public async Task ClearEmployeeRestaurantAsync() => await sessionStorage.DeleteAsync(EmployeeContextKey);

    public async Task<Guid?> GetCustomerRestaurantIdAsync()
    {
        ProtectedBrowserStorageResult<Guid> result = await sessionStorage.GetAsync<Guid>(CustomerContextKey);
        return result.Success && result.Value != Guid.Empty ? result.Value : null;
    }

    public async Task SetCustomerRestaurantIdAsync(Guid restaurantId)
    {
        EnsureValidId(restaurantId, nameof(restaurantId));
        await sessionStorage.SetAsync(CustomerContextKey, restaurantId);
    }

    private static void EnsureValidId(Guid id, string parameterName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Identifier must not be empty.", parameterName);
        }
    }

    private sealed record EmployeeContext(Guid UserId, Guid RestaurantId);
}
