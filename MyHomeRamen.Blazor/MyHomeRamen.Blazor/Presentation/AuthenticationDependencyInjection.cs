using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MyHomeRamen.Blazor.Presentation.Authentication;

namespace MyHomeRamen.Blazor.Presentation;

internal static class AuthenticationDependencyInjection
{
    internal static IServiceCollection AddAuthenticationHandlers(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>()
                .AddTransient<AdminAuthHeaderHandler>();

        return services;
    }

    internal static IServiceCollection AddKeycloackAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        string infrastructurePrefix = builder.Configuration["RestaurantConfiguration:InfrastructurePrefix"]!;

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddKeycloakOpenIdConnect(
                    serviceName: $"{infrastructurePrefix}-key-cloak",
                    realm: builder.Configuration["Authorization:Realm"]!,
                    options =>
                    {
                        options.ClientId = builder.Configuration["Authentication:Blazor:ClientId"];
                        options.ClientSecret = builder.Configuration["Authentication:Blazor:ClientSecret"];
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.Scope.Add("openid");
                        options.Scope.Add("profile");
                        options.Scope.Add("my-home-ramen-scope");
                        options.SaveTokens = true;
                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        if (builder.Environment.IsDevelopment())
                        {
                            options.RequireHttpsMetadata = false;
                        }
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        return services;
    }
}
