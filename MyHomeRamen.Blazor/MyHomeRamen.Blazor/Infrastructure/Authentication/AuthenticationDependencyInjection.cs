using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MyHomeRamen.Blazor.Features.Account.Common.Models;
using MyHomeRamen.Blazor.Features.Account.Common.Services;
using MyHomeRamen.Blazor.Features.Restaurants.Shared;
using MyHomeRamen.Blazor.Infrastructure.Authentication.HeaderHandlers;
using MyHomeRamen.Blazor.Infrastructure.Authentication.StateProvider;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication;

internal static class AuthenticationDependencyInjection
{
    internal static IServiceCollection AddAuthenticationHandlers(this IServiceCollection services)
    {
        services.AddTransient<AuthHeaderHandler>()
                .AddTransient<AdminAuthHeaderHandler>();

        services.AddScoped<CustomAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

        return services;
    }

    internal static IServiceCollection AddBlazorAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(PermissionConstants.CompanyView, policy =>
                    policy.RequireClaim(AuthenticationClaimConstants.OwnerCanViewPanel, bool.TrueString))
                .AddPolicy(PermissionConstants.CompanySocialMediaView, policy =>
                    policy.RequireClaim(AuthenticationClaimConstants.OwnerCanViewPanel, bool.TrueString))
                .AddPolicy(PermissionConstants.CompanySocialMediaEdit, policy =>
                    policy.RequireClaim(AuthenticationClaimConstants.OwnerCanViewPanel, bool.TrueString));

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
                            OnTokenValidated = async ctx =>
                            {
                                string? rawAccessToken = ctx.TokenEndpointResponse?.AccessToken;
                                if (string.IsNullOrEmpty(rawAccessToken))
                                {
                                    return;
                                }

                                JsonWebToken accessToken = new(rawAccessToken);
                                ClaimsIdentity identity = (ClaimsIdentity)ctx.Principal!.Identity!;

                                foreach (string claimType in (string[])["resource_access"])
                                {
                                    Claim? claim = accessToken.Claims.FirstOrDefault(c => c.Type == claimType);
                                    if (claim is not null && !identity.HasClaim(c => c.Type == claimType))
                                    {
                                        identity.AddClaim(claim);
                                    }
                                }

                                CustomerAccountApiClient accountApiClient = ctx.HttpContext.RequestServices
                                    .GetRequiredService<CustomerAccountApiClient>();

                                GetMeModel currentUser = await accountApiClient.GetMeAsync(ctx.HttpContext.RequestAborted, rawAccessToken);

                                identity.AddClaim(new Claim(AuthenticationClaimConstants.DomainId, currentUser.UserId.ToString()));

                                if (currentUser.FirstName is not null)
                                {
                                    identity.AddClaim(new Claim(AuthenticationClaimConstants.FirstName, currentUser.FirstName));
                                }

                                if (currentUser.AdminActions?.CanViewPanel == true)
                                {
                                    identity.AddClaim(new Claim(AuthenticationClaimConstants.AdminCanViewPanel, bool.TrueString));
                                }

                                if (currentUser.OwnerActions?.CanViewPanel == true)
                                {
                                    identity.AddClaim(new Claim(AuthenticationClaimConstants.OwnerCanViewPanel, bool.TrueString));
                                }
                            }
                        };
                    })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        return services;
    }
}
