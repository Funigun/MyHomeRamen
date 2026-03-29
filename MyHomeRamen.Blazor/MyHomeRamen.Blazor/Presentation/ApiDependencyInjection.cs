using MyHomeRamen.Blazor.Features.Account;
using MyHomeRamen.Blazor.Features.Admin.Employees;
using MyHomeRamen.Blazor.Features.Menu.Common.Services;
using MyHomeRamen.Blazor.Presentation.Authentication;
using MyHomeRamen.ServiceDefaults;

namespace MyHomeRamen.Blazor.Presentation;

internal static class ApiDependencyInjection
{
    internal static IServiceCollection AddApiServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddHttpClient<CustomerAccountApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.IdentityApi(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<EmployeeApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.IdentityApi(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AdminAuthHeaderHandler>();

        services.AddHttpClient<MenuApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{ServiceNames.Api(infrastructurePrefix)}");
            }
        ).AddHttpMessageHandler<AdminAuthHeaderHandler>();

        return services;
    }
}
