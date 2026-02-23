using MyHomeRamen.Blazor.Features.Admin.Employees;
using MyHomeRamen.Blazor.Presentation.Authentication;

namespace MyHomeRamen.Blazor.Presentation;

internal static class ApiDependencyInjection
{
    internal static IServiceCollection AddApiServices(this IServiceCollection services, string infrastructurePrefix)
    {
        services.AddHttpClient<EmployeeApiClient>(client =>
            {
                client.BaseAddress = new Uri($"https+http://{infrastructurePrefix}-identity-api");
            }
        ).AddHttpMessageHandler<AdminAuthHeaderHandler>();

        return services;
    }
}
