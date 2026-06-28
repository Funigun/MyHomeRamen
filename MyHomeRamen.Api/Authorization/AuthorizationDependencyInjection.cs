using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Features.Common.Configurations;
using Scalar.AspNetCore;

namespace MyHomeRamen.Api.Authorization;

internal static class AuthorizationDependencyInjection
{
    internal const string RestaurantCustomerPolicy = "RestaurantCustomer";
    internal const string RestaurantEmployeePolicy = "RestaurantEmployee";
    internal const string RestaurantManagerPolicy = "RestaurantManager";
    internal const string AnyAuthenticatedPolicy = "AnyAuthenticated";

    internal static ScalarOptions ConfigureScalarOptions(this ScalarOptions options, RestaurantConfigurationProvider configurationProvider)
    {
        options.WithTitle(configurationProvider.RestaurantName)
               .WithTheme(ScalarTheme.Kepler)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        options.AddPreferredSecuritySchemes(RestaurantCustomerPolicy, RestaurantEmployeePolicy, RestaurantManagerPolicy);

        options.Servers = [new("https://localhost:7460")];

        return options;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(RestaurantCustomerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantCustomerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("MenuCustomer"))

                .AddPolicy(RestaurantEmployeePolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantEmployeePolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("MenuEmployee"))

                .AddPolicy(RestaurantManagerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantManagerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("MenuAdmin"))

                .AddPolicy(AnyAuthenticatedPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantCustomerPolicy, RestaurantEmployeePolicy, RestaurantManagerPolicy)
                          .RequireAuthenticatedUser());

        return services;
    }

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, AuthorizationConfiguration authConfig)
    {
        string[] validIssuers = [authConfig.Issuer];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(RestaurantCustomerPolicy, options =>
                {
                    options.Authority = authConfig.Authority;
                    options.Audience = authConfig.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantEmployeePolicy, options =>
                {
                    options.Authority = authConfig.Authority;
                    options.Audience = authConfig.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantManagerPolicy, options =>
                {
                    options.Authority = authConfig.Authority;
                    options.Audience = authConfig.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddPolicyScheme(JwtBearerDefaults.AuthenticationScheme, null, options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        string? scheme = context.Request.Headers["x-scheme"].FirstOrDefault();

                        return scheme switch
                        {
                            RestaurantManagerPolicy => RestaurantManagerPolicy,
                            RestaurantEmployeePolicy => RestaurantEmployeePolicy,
                            RestaurantCustomerPolicy => RestaurantCustomerPolicy,
                            _ => RestaurantCustomerPolicy
                        };
                    };
                });

        services.AddTransient<IClaimsTransformation, KeycloakClaimsTransformation>();

        return services;
    }
}
