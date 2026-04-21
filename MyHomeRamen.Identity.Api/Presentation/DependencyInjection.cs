using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyHomeRamen.Api.Common.Configuration;
using Scalar.AspNetCore;

namespace MyHomeRamen.Identity.Api.Presentation;

internal static class DependencyInjection
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

        options.Servers = [new("https://localhost:7188")];

        return options;
    }

    internal static IServiceCollection ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
                .AddPolicy(RestaurantCustomerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantCustomerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("customer"))

                .AddPolicy(RestaurantEmployeePolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantEmployeePolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("employee"))

                .AddPolicy(RestaurantManagerPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantManagerPolicy)
                          .RequireAuthenticatedUser()
                          .RequireRole("manager"))

                .AddPolicy(AnyAuthenticatedPolicy, policy =>
                    policy.AddAuthenticationSchemes(RestaurantCustomerPolicy, RestaurantEmployeePolicy, RestaurantManagerPolicy)
                          .RequireAuthenticatedUser());

        return services;
    }

    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, AuthorizationConfiguration configuration)
    {
        string[] validIssuers = [configuration.Issuer];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(RestaurantCustomerPolicy, options =>
                {
                    options.Authority = configuration.Authority;
                    options.Audience = configuration.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantEmployeePolicy, options =>
                {
                    options.Authority = configuration.Authority;
                    options.Audience = configuration.Audience;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuers = validIssuers,
                    };
                })
                .AddJwtBearer(RestaurantManagerPolicy, options =>
                {
                    options.Authority = configuration.Authority;
                    options.Audience = configuration.Audience;
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

        services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();

        return services;
    }
}
