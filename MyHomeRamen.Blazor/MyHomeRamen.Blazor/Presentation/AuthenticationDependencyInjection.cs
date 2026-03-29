using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MyHomeRamen.Blazor.Presentation.Authentication;

namespace MyHomeRamen.Blazor.Presentation;

internal static class AuthenticationDependencyInjection
{
    internal static IServiceCollection AddAuthenticationHandlers(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>()
                .AddTransient<AdminAuthHeaderHandler>();

        services.AddScoped<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        services.AddScoped<CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

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
                        options.Scope.Add("menu");
                        options.SaveTokens = true;
                        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        if (builder.Environment.IsDevelopment())
                        {
                            options.RequireHttpsMetadata = false;
                        }

                        options.Events = new OpenIdConnectEvents
                        {
                            OnTokenValidated = ctx =>
                            {
                                string? rawAccessToken = ctx.TokenEndpointResponse?.AccessToken;
                                if (string.IsNullOrEmpty(rawAccessToken))
                                {
                                    return Task.CompletedTask;
                                }

                                JsonWebToken accessToken = new(rawAccessToken);
                                ClaimsIdentity identity = (ClaimsIdentity)ctx.Principal!.Identity!;

                                foreach (string claimType in (string[])["resource_access", "realm_access"])
                                {
                                    Claim? claim = accessToken.Claims.FirstOrDefault(c => c.Type == claimType);
                                    if (claim is not null && !identity.HasClaim(c => c.Type == claimType))
                                    {
                                        identity.AddClaim(claim);
                                    }
                                }

                                return Task.CompletedTask;
                            }
                        };
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        return services;
    }
}
